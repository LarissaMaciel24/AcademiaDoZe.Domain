using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

//Larissa Maciel
public class AcessoService : IAcessoService
{
    private const int TipoAluno = 0;
    private const int TipoColaborador = 1;

    private readonly Func<IAcessoRepository> _acessoRepoFactory;
    private readonly Func<IAlunoRepository> _alunoRepoFactory;
    private readonly Func<IColaboradorRepository> _colaboradorRepoFactory;
    private readonly Func<IMatriculaRepository> _matriculaRepoFactory;

    public AcessoService(
        Func<IAcessoRepository> acessoRepoFactory,
        Func<IAlunoRepository> alunoRepoFactory,
        Func<IColaboradorRepository> colaboradorRepoFactory,
        Func<IMatriculaRepository> matriculaRepoFactory)
    {
        _acessoRepoFactory = acessoRepoFactory ?? throw new ArgumentNullException(nameof(acessoRepoFactory));
        _alunoRepoFactory = alunoRepoFactory ?? throw new ArgumentNullException(nameof(alunoRepoFactory));
        _colaboradorRepoFactory = colaboradorRepoFactory ?? throw new ArgumentNullException(nameof(colaboradorRepoFactory));
        _matriculaRepoFactory = matriculaRepoFactory ?? throw new ArgumentNullException(nameof(matriculaRepoFactory));
    }

    // Data/hora atual sem os milésimos (o banco guarda só até segundos)
    private static DateTime Agora()
    {
        var n = DateTime.Now;
        return new DateTime(n.Ticks - (n.Ticks % TimeSpan.TicksPerSecond));
    }

    private static string Formatar(TimeSpan tempo)
    {
        if (tempo < TimeSpan.Zero)
            tempo = TimeSpan.Zero;

        return $"{(int)tempo.TotalHours:00}:{tempo.Minutes:00}:{tempo.Seconds:00}";
    }

    // Registros do dia em pares: 1º = entrada, 2º = saída, 3º = entrada...
    // Se sobrar uma entrada sem saída, conta até "agora".
    private static TimeSpan SomarPermanencia(List<DateTime> registros, DateTime agora)
    {
        var total = TimeSpan.Zero;

        for (int i = 0; i < registros.Count; i += 2)
        {
            var entrada = registros[i];
            var saida = i + 1 < registros.Count ? registros[i + 1] : agora;
            total += saida - entrada;
        }

        return total;
    }

    public async Task<bool> AlunoEstaDentroAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var registros = await _acessoRepoFactory().ObterDoDiaPorPessoaAsync(TipoAluno, alunoId, hoje, cancellationToken);
        return registros.Count() % 2 == 1;
    }

    public async Task<bool> ColaboradorEstaDentroAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var registros = await _acessoRepoFactory().ObterDoDiaPorPessoaAsync(TipoColaborador, colaboradorId, hoje, cancellationToken);
        return registros.Count() % 2 == 1;
    }

    public async Task<AcessoResultadoDto> RegistrarAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        _ = await _alunoRepoFactory().ObterPorId(alunoId, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

        var acessoRepo = _acessoRepoFactory();
        var agora = Agora();
        var hoje = DateOnly.FromDateTime(agora);

        var registros = (await acessoRepo.ObterDoDiaPorPessoaAsync(TipoAluno, alunoId, hoje, cancellationToken)).ToList();
        bool estaDentro = registros.Count % 2 == 1;

        // SAÍDA
        if (estaDentro)
        {
            var permanencia = agora - registros[^1];
            await acessoRepo.RegistrarAsync(TipoAluno, alunoId, agora, cancellationToken);

            return new AcessoResultadoDto
            {
                Sucesso = true,
                Titulo = "Saída Registrada",
                Mensagem = $"Saída registrada. Tempo de permanência: {Formatar(permanencia)}.",
                Dentro = false
            };
        }

        // ENTRADA: precisa ter matrícula ativa
        var matricula = await _matriculaRepoFactory().ObterMatriculaAtivaPorAluno(alunoId, cancellationToken);

        if (matricula == null)
        {
            return new AcessoResultadoDto
            {
                Sucesso = false,
                Titulo = "Acesso Negado",
                Mensagem = "Acesso negado: o aluno não possui matrícula ativa.",
                Dentro = false
            };
        }

        int diasRestantes = Math.Max(0, matricula.DataFinal.DayNumber - hoje.DayNumber);
        await acessoRepo.RegistrarAsync(TipoAluno, alunoId, agora, cancellationToken);

        return new AcessoResultadoDto
        {
            Sucesso = true,
            Titulo = "Entrada Autorizada",
            Mensagem = $"Entrada autorizada. Dias restantes de plano: {diasRestantes} dia(s).",
            Dentro = true
        };
    }

    public async Task<AcessoResultadoDto> RegistrarColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradorRepoFactory().ObterPorId(colaboradorId, cancellationToken)
            ?? throw new InvalidOperationException($"Colaborador com ID {colaboradorId} não encontrado.");

        var acessoRepo = _acessoRepoFactory();
        var agora = Agora();
        var hoje = DateOnly.FromDateTime(agora);

        var registros = (await acessoRepo.ObterDoDiaPorPessoaAsync(TipoColaborador, colaboradorId, hoje, cancellationToken)).ToList();
        bool estaDentro = registros.Count % 2 == 1;

        // SAÍDA: soma todos os registros do dia
        if (estaDentro)
        {
            await acessoRepo.RegistrarAsync(TipoColaborador, colaboradorId, agora, cancellationToken);

            registros.Add(agora);
            var totalDoDia = SomarPermanencia(registros, agora);

            return new AcessoResultadoDto
            {
                Sucesso = true,
                Titulo = "Saída Registrada",
                Mensagem = $"Saída registrada. Tempo de permanência hoje: {Formatar(totalDoDia)}.",
                Dentro = false
            };
        }

        // ENTRADA: valida o limite diário (8h CLT / 6h Estágio)
        bool ehClt = colaborador.Vinculo == ColaboradorVinculo.CLT;
        var limite = TimeSpan.FromHours(ehClt ? 8 : 6);
        var trabalhado = SomarPermanencia(registros, agora);

        if (trabalhado >= limite)
        {
            string vinculo = ehClt ? "CLT" : "estagiário";

            return new AcessoResultadoDto
            {
                Sucesso = false,
                Titulo = "Acesso Negado",
                Mensagem = $"Acesso negado: o colaborador ({vinculo}) já atingiu o limite diário de {limite.TotalHours:0} horas.",
                Dentro = false
            };
        }

        await acessoRepo.RegistrarAsync(TipoColaborador, colaboradorId, agora, cancellationToken);

        return new AcessoResultadoDto
        {
            Sucesso = true,
            Titulo = "Entrada Registrada",
            Mensagem = $"Entrada registrada. Horas trabalhadas hoje até o momento: {Formatar(trabalhado)}.",
            Dentro = true
        };
    }

    public async Task<IEnumerable<AcessoRegistroDto>> ObterRegistrosDeHojeAsync(CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var todos = await _acessoRepoFactory().ObterDoDiaAsync(hoje, cancellationToken);

        var alunoRepo = _alunoRepoFactory();
        var colaboradorRepo = _colaboradorRepoFactory();
        var lista = new List<AcessoRegistroDto>();

        foreach (var grupo in todos.GroupBy(r => new { r.PessoaTipo, r.PessoaId }))
        {
            bool ehAluno = grupo.Key.PessoaTipo == TipoAluno;

            string nome = ehAluno
                ? (await alunoRepo.ObterPorId(grupo.Key.PessoaId, cancellationToken))?.Nome ?? $"Aluno {grupo.Key.PessoaId}"
                : (await colaboradorRepo.ObterPorId(grupo.Key.PessoaId, cancellationToken))?.Nome ?? $"Colaborador {grupo.Key.PessoaId}";

            var eventos = grupo.OrderBy(r => r.DataHora).ToList();
            var acumulado = TimeSpan.Zero;

            for (int i = 0; i < eventos.Count; i++)
            {
                string mensagem;

                if (i % 2 == 0) // entrada
                {
                    mensagem = ehAluno
                        ? "Entrada registrada."
                        : $"Entrada registrada. Horas trabalhadas hoje até o momento: {Formatar(acumulado)}.";
                }
                else // saída
                {
                    var permanencia = eventos[i].DataHora - eventos[i - 1].DataHora;
                    acumulado += permanencia;

                    mensagem = ehAluno
                        ? $"Saída registrada. Tempo de permanência: {Formatar(permanencia)}."
                        : $"Saída registrada. Tempo de permanência hoje: {Formatar(acumulado)}.";
                }

                lista.Add(new AcessoRegistroDto
                {
                    Nome = nome,
                    Mensagem = mensagem,
                    DataHora = eventos[i].DataHora
                });
            }
        }

        return lista.OrderByDescending(r => r.DataHora).ToList();
    }
}