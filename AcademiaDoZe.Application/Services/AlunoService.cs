using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

//Larissa Maciel
public class AlunoService : IAlunoService
{
    private readonly Func<IAlunoRepository> _repoFactory;
    private readonly Func<ILogradouroRepository> _logradouroRepoFactory;

    public AlunoService(Func<IAlunoRepository> repoFactory, Func<ILogradouroRepository> logradouroRepoFactory)
    {
        _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        _logradouroRepoFactory = logradouroRepoFactory ?? throw new ArgumentNullException(nameof(logradouroRepoFactory));
    }

    public async Task<bool> CpfJaExisteAsync(string cpf, int? id = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        var cpfResult = Cpf.Criar(cpf);
        if (!cpfResult.Sucesso) return false;
        return await _repoFactory().CpfJaExiste(cpfResult.Valor!, id, cancellationToken);
    }

    public async Task<bool> EmailJaExisteAsync(string email, int? id = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var emailResult = Email.Criar(email);
        if (!emailResult.Sucesso) return false;
        return await _repoFactory().EmailJaExiste(emailResult.Valor!, id, cancellationToken);
    }

    public async Task<AlunoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var aluno = await _repoFactory().ObterPorId(id, cancellationToken);
        return aluno?.ToDto();
    }

    public async Task<IEnumerable<AlunoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var alunos = await _repoFactory().ObterTodos(cancellationToken);
        return [.. alunos.Select(a => a.ToDto())];
    }

    public async Task<AlunoDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF não pode ser vazio.", nameof(cpf));

        var cpfResult = Cpf.Criar(cpf);
        if (!cpfResult.Sucesso)
            throw new ArgumentException($"CPF inválido: {string.Join(", ", cpfResult.Notificacoes.Select(n => n.Mensagem))}", nameof(cpf));

        var aluno = await _repoFactory().ObterPorCpf(cpfResult.Valor!, cancellationToken);
        return aluno?.ToDto();
    }

    public async Task<AlunoDto?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        var emailResult = Email.Criar(email);
        if (!emailResult.Sucesso)
            throw new ArgumentException($"Email inválido: {string.Join(", ", emailResult.Notificacoes.Select(n => n.Mensagem))}", nameof(email));

        var aluno = await _repoFactory().ObterPorEmail(emailResult.Valor!, cancellationToken);
        return aluno?.ToDto();
    }

    // IAlunoRepository não tem um ObterPorNome, então filtramos em memória sobre ObterTodos
    public async Task<IEnumerable<AlunoDto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));

        var alunos = await _repoFactory().ObterTodos(cancellationToken);
        return [.. alunos
            .Where(a => a.Nome.Contains(nome.Trim(), StringComparison.OrdinalIgnoreCase))
            .Select(a => a.ToDto())];
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var aluno = await _repoFactory().ObterPorId(id, cancellationToken);
        if (aluno == null) return false;
        return await _repoFactory().Remover(id, cancellationToken);
    }

    // IAlunoRepository não tem TrocarSenha: reconstruímos o Aluno com a nova senha (hash) e salvamos
    public async Task<bool> TrocarSenhaAsync(int id, string novaSenha, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(novaSenha))
            throw new ArgumentException("Nova senha não pode ser vazia.", nameof(novaSenha));

        var validacaoSenha = Senha.Criar(novaSenha);
        if (!validacaoSenha.Sucesso)
            throw new ArgumentException($"Nova senha inválida: {string.Join(", ", validacaoSenha.Notificacoes.Select(n => n.Mensagem))}", nameof(novaSenha));

        var aluno = await _repoFactory().ObterPorId(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Aluno com ID {id} não encontrado.");

        var alunoDto = aluno.ToDto();
        alunoDto.Senha = PasswordHasher.Hash(novaSenha);

        var alunoAtualizado = aluno.UpdateFromDto(alunoDto, aluno.Endereco.Logradouro);
        await _repoFactory().Atualizar(alunoAtualizado, cancellationToken);
        return true;
    }

    public async Task<AlunoDto> AdicionarAsync(AlunoDto alunoDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(alunoDto);

        var cpfResult = Cpf.Criar(alunoDto.Cpf);
        if (!cpfResult.Sucesso)
            throw new ArgumentException($"CPF inválido: {string.Join(", ", cpfResult.Notificacoes.Select(n => n.Mensagem))}", nameof(alunoDto));

        if (await _repoFactory().CpfJaExiste(cpfResult.Valor!, null, cancellationToken))
            throw new InvalidOperationException($"Já existe um aluno cadastrado com o CPF {alunoDto.Cpf}.");

        if (!string.IsNullOrWhiteSpace(alunoDto.Email))
        {
            var emailResult = Email.Criar(alunoDto.Email);
            if (!emailResult.Sucesso)
                throw new ArgumentException($"Email inválido: {string.Join(", ", emailResult.Notificacoes.Select(n => n.Mensagem))}", nameof(alunoDto));

            if (await _repoFactory().EmailJaExiste(emailResult.Valor!, null, cancellationToken))
                throw new InvalidOperationException($"Já existe um aluno cadastrado com o Email {alunoDto.Email}.");
        }

        if (!string.IsNullOrWhiteSpace(alunoDto.Senha))
        {
            var senhaValidacao = Senha.Criar(alunoDto.Senha);
            if (!senhaValidacao.Sucesso)
                throw new ArgumentException($"Senha não atende aos requisitos mínimos: {string.Join(", ", senhaValidacao.Notificacoes.Select(n => n.Mensagem))}", nameof(alunoDto));

            alunoDto.Senha = PasswordHasher.Hash(alunoDto.Senha);
        }

        if (alunoDto.Endereco == null || alunoDto.Endereco.Id <= 0)
            throw new InvalidOperationException("É necessário informar um logradouro válido para o endereço do aluno.");

        var logradouro = await _logradouroRepoFactory().ObterPorId(alunoDto.Endereco.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Logradouro com ID {alunoDto.Endereco.Id} não encontrado.");

        var aluno = alunoDto.ToEntity(logradouro);
        var adicionado = await _repoFactory().Adicionar(aluno, cancellationToken);
        return adicionado.ToDto();
    }

    public async Task<AlunoDto> AtualizarAsync(AlunoDto alunoDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(alunoDto);

        var alunoExistente = await _repoFactory().ObterPorId(alunoDto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Aluno com ID {alunoDto.Id} não encontrado.");

        var cpfResult = Cpf.Criar(alunoDto.Cpf);
        if (!cpfResult.Sucesso)
            throw new ArgumentException($"CPF inválido: {string.Join(", ", cpfResult.Notificacoes.Select(n => n.Mensagem))}", nameof(alunoDto));

        if (await _repoFactory().CpfJaExiste(cpfResult.Valor!, alunoDto.Id, cancellationToken))
            throw new InvalidOperationException($"Já existe outro aluno cadastrado com o CPF {alunoDto.Cpf}.");

        if (!string.IsNullOrWhiteSpace(alunoDto.Email) && !string.Equals(alunoDto.Email, alunoExistente.Email.Valor, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = Email.Criar(alunoDto.Email);
            if (!emailResult.Sucesso)
                throw new ArgumentException($"Email inválido: {string.Join(", ", emailResult.Notificacoes.Select(n => n.Mensagem))}", nameof(alunoDto));

            if (await _repoFactory().EmailJaExiste(emailResult.Valor!, alunoDto.Id, cancellationToken))
                throw new InvalidOperationException($"Já existe outro aluno cadastrado com o Email {alunoDto.Email}.");
        }

        if (!string.IsNullOrWhiteSpace(alunoDto.Senha))
        {
            var senhaValidacao = Senha.Criar(alunoDto.Senha);
            if (!senhaValidacao.Sucesso)
                throw new ArgumentException($"Senha não atende aos requisitos mínimos: {string.Join(", ", senhaValidacao.Notificacoes.Select(n => n.Mensagem))}", nameof(alunoDto));

            alunoDto.Senha = PasswordHasher.Hash(alunoDto.Senha);
        }

        int logradouroId = (alunoDto.Endereco != null && alunoDto.Endereco.Id > 0)
            ? alunoDto.Endereco.Id
            : alunoExistente.Endereco.Logradouro.Id;

        var logradouro = await _logradouroRepoFactory().ObterPorId(logradouroId, cancellationToken)
            ?? throw new KeyNotFoundException($"Logradouro com ID {logradouroId} não encontrado.");

        var alunoAtualizado = alunoExistente.UpdateFromDto(alunoDto, logradouro);
        var atualizado = await _repoFactory().Atualizar(alunoAtualizado, cancellationToken);
        return atualizado.ToDto();
    }
}