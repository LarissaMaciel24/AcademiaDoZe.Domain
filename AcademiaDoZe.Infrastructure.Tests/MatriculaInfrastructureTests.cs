using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

// Larissa Maciel
public class MatriculaInfrastructureTests : TestBase
{
    private readonly MatriculaRepository _repository;
    private readonly AlunoRepository _alunoRepository;
    private readonly LogradouroRepository _logradouroRepository;

    public MatriculaInfrastructureTests()
    {
        _repository = new MatriculaRepository(
            ConnectionString,
            DatabaseType);

        _alunoRepository = new AlunoRepository(
            ConnectionString,
            DatabaseType);

        _logradouroRepository = new LogradouroRepository(
            ConnectionString,
            DatabaseType);
    }

    private async Task<Aluno> CriarEInserirAlunoAsync()
    {
        var cep = GerarCep();

        var logradouroResult = Logradouro.Criar(
            id: 0,
            pais: "Brasil",
            estado: "SC",
            cidade: "Lages",
            bairro: "Centro",
            nome: "Rua Teste",
            cep: cep);

        Assert.True(logradouroResult.Sucesso);
        Assert.NotNull(logradouroResult.Valor);

        var logradouro = await _logradouroRepository.Adicionar(
            logradouroResult.Valor!);

        var enderecoResult = Endereco.Criar(
            cep: logradouro.Cep,
            logradouro: logradouro,
            numero: "100",
            complemento: "Casa");

        Assert.True(enderecoResult.Sucesso);
        Assert.NotNull(enderecoResult.Valor);

        var cpfResult = Cpf.Criar(GerarCpf());
        var telefoneResult = Telefone.Criar(GerarTelefone());
        var emailResult = Email.Criar(GerarEmail());
        var senhaResult = Senha.Criar("sqlserver");

        var fotoResult = Arquivo.Criar(
            "foto",
            "jpg",
            new byte[] { 1, 2, 3 });

        Assert.True(cpfResult.Sucesso);
        Assert.True(telefoneResult.Sucesso);
        Assert.True(emailResult.Sucesso);
        Assert.True(senhaResult.Sucesso);
        Assert.True(fotoResult.Sucesso);

        var alunoResult = Aluno.Criar(
            id: 0,
            nome: "Aluno Teste",
            cpf: cpfResult.Valor!,
            dataNascimento: new DateOnly(2000, 1, 1),
            telefone: telefoneResult.Valor!,
            email: emailResult.Valor!,
            endereco: enderecoResult.Valor!,
            senha: senhaResult.Valor!,
            foto: fotoResult.Valor!);

        Assert.True(alunoResult.Sucesso);
        Assert.NotNull(alunoResult.Valor);

        return await _alunoRepository.Adicionar(
            alunoResult.Valor!);
    }

    private async Task<Matricula> CriarEInserirMatriculaAsync(
        Aluno? aluno = null,
        MatriculaPlano plano = MatriculaPlano.Mensal,
        int diasParaVencimento = 30)
    {
        aluno ??= await CriarEInserirAlunoAsync();

        var laudoResult = Arquivo.Criar(
            "laudo_medico",
            "pdf",
            new byte[] { 1, 2, 3, 4 });

        Assert.True(laudoResult.Sucesso);
        Assert.NotNull(laudoResult.Valor);

        var dataInicio = DateOnly.FromDateTime(
            DateTime.Today);

        var dataFinal = dataInicio.AddDays(
            diasParaVencimento);

        var matriculaResult = Matricula.Criar(
            id: 0,
            aluno: aluno,
            plano: plano,
            dataInicio: dataInicio,
            dataFinal: dataFinal,
            objetivo: "Larissa Maciel",
            restricoes: MatriculaRestricoes.Nenhuma,
            observacoes: "Nenhuma observação",
            laudoMedico: laudoResult.Valor!);

        Assert.True(matriculaResult.Sucesso);
        Assert.NotNull(matriculaResult.Valor);

        return await _repository.Adicionar(
            matriculaResult.Valor!);
    }

    [Fact]
    public async Task Matricula_Adicionar_E_ObterPorId_Sucesso()
    {
        var matricula =
            await CriarEInserirMatriculaAsync();

        Assert.NotNull(matricula);
        Assert.True(matricula.Id > 0);

        var obtida =
            await _repository.ObterPorId(
                matricula.Id);

        Assert.NotNull(obtida);

        Assert.Equal(
            matricula.Id,
            obtida.Id);

        Assert.Equal(
            matricula.Aluno.Id,
            obtida.Aluno.Id);

        Assert.Equal(
            matricula.Plano,
            obtida.Plano);

        Assert.Equal(
            matricula.Objetivo,
            obtida.Objetivo);
    }

    [Fact]
    public async Task Matricula_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtida =
            await _repository.ObterPorId(999999);

        Assert.Null(obtida);
    }

    [Fact]
    public async Task Matricula_ObterTodos_Sucesso()
    {
        await CriarEInserirMatriculaAsync();

        var matriculas =
            await _repository.ObterTodos();

        Assert.NotNull(matriculas);
        Assert.NotEmpty(matriculas);
    }

    [Fact]
    public async Task Matricula_Atualizar_Sucesso()
    {
        var matricula =
            await CriarEInserirMatriculaAsync();

        matricula.AlterarObjetivo(
            "Objetivo atualizado");

        matricula.AlterarObservacoes(
            "Observação atualizada");

        var atualizada =
            await _repository.Atualizar(
                matricula);

        Assert.NotNull(atualizada);

        Assert.Equal(
            "Objetivo atualizado",
            atualizada.Objetivo);

        Assert.Equal(
            "Observação atualizada",
            atualizada.Observacoes);

        var noBanco =
            await _repository.ObterPorId(
                matricula.Id);

        Assert.NotNull(noBanco);

        Assert.Equal(
            "Objetivo atualizado",
            noBanco.Objetivo);

        Assert.Equal(
            "Observação atualizada",
            noBanco.Observacoes);
    }

    [Fact]
    public async Task Matricula_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var laudoResult = Arquivo.Criar(
            "laudo_medico",
            "pdf",
            new byte[] { 1, 2, 3 });

        Assert.True(laudoResult.Sucesso);
        Assert.NotNull(laudoResult.Valor);

        var matriculaResult = Matricula.Criar(
            id: 999999,
            aluno: aluno,
            plano: MatriculaPlano.Mensal,
            dataInicio: DateOnly.FromDateTime(DateTime.Today),
            dataFinal: DateOnly.FromDateTime(DateTime.Today).AddDays(30),
            objetivo: "Matrícula inexistente",
            restricoes: MatriculaRestricoes.Nenhuma,
            observacoes: "Teste",
            laudoMedico: laudoResult.Valor!);

        Assert.True(matriculaResult.Sucesso);
        Assert.NotNull(matriculaResult.Valor);

        var ex =
            await Assert.ThrowsAsync<InfrastructureException>(
                () => _repository.Atualizar(
                    matriculaResult.Valor!));

        Assert.Equal(
            "REGISTRO_NAO_ENCONTRADO",
            ex.ErrorCode);
    }

    [Fact]
    public async Task Matricula_Remover_Sucesso()
    {
        var matricula =
            await CriarEInserirMatriculaAsync();

        var removida =
            await _repository.Remover(
                matricula.Id);

        Assert.True(removida);

        var noBanco =
            await _repository.ObterPorId(
                matricula.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Matricula_Remover_RetornaFalseQuandoInexistente()
    {
        var removida =
            await _repository.Remover(999999);

        Assert.False(removida);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var matricula =
            await CriarEInserirMatriculaAsync(
                aluno);

        var matriculas =
            await _repository.ObterPorAluno(
                aluno.Id);

        Assert.NotNull(matriculas);
        Assert.Contains(
            matriculas,
            m => m.Id == matricula.Id);

        Assert.All(
            matriculas,
            m => Assert.Equal(
                aluno.Id,
                m.Aluno.Id));
    }

    [Fact]
    public async Task Matricula_ObterMatriculaAtivaPorAluno_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var matricula =
            await CriarEInserirMatriculaAsync(
                aluno,
                MatriculaPlano.Mensal,
                30);

        var ativa =
            await _repository.ObterMatriculaAtivaPorAluno(
                aluno.Id);

        Assert.NotNull(ativa);

        Assert.Equal(
            matricula.Id,
            ativa.Id);

        Assert.Equal(
            aluno.Id,
            ativa.Aluno.Id);
    }

    [Fact]
    public async Task Matricula_PossuiMatriculaAtiva_RetornaTrue()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Mensal,
            30);

        var possui =
            await _repository.PossuiMatriculaAtiva(
                aluno.Id);

        Assert.True(possui);
    }

    [Fact]
    public async Task Matricula_ObterAtivas_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var matricula =
            await CriarEInserirMatriculaAsync(
                aluno,
                MatriculaPlano.Trimestral,
                30);

        var ativas =
            await _repository.ObterAtivas(
                aluno.Id);

        Assert.NotNull(ativas);

        Assert.Contains(
            ativas,
            m => m.Id == matricula.Id);

        Assert.All(
            ativas,
            m => Assert.True(
                m.DataFinal >=
                DateOnly.FromDateTime(
                    DateTime.Today)));
    }

    [Fact]
    public async Task Matricula_ObterVencendoEmDias_Sucesso()
    {
        var matricula =
            await CriarEInserirMatriculaAsync(
                plano: MatriculaPlano.Mensal,
                diasParaVencimento: 5);

        var matriculas =
            await _repository.ObterVencendoEmDias(7);

        Assert.NotNull(matriculas);

        Assert.Contains(
            matriculas,
            m => m.Id == matricula.Id);
    }

    [Fact]
    public async Task Matricula_ObterPorPlano_Sucesso()
    {
        var matricula =
            await CriarEInserirMatriculaAsync(
                plano: MatriculaPlano.Semestral,
                diasParaVencimento: 60);

        var matriculas =
            await _repository.ObterPorPlano(
                MatriculaPlano.Semestral);

        Assert.NotNull(matriculas);

        Assert.Contains(
            matriculas,
            m => m.Id == matricula.Id);

        Assert.All(
            matriculas,
            m => Assert.Equal(
                MatriculaPlano.Semestral,
                m.Plano));
    }
}