using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

// Larissa Maciel
public class AlunoInfrastructureTests : TestBase
{
    private readonly AlunoRepository _repository;
    private readonly LogradouroRepository _logradouroRepository;

    public AlunoInfrastructureTests()
    {
        _repository = new AlunoRepository(
            ConnectionString,
            DatabaseType);

        _logradouroRepository = new LogradouroRepository(
            ConnectionString,
            DatabaseType);
    }

    private async Task<Logradouro> CriarEInserirLogradouroAsync()
    {
        var cep = GerarCep();

        var logradouroResult = Logradouro.Criar(
            id: 0,
            pais: "Brasil",
            estado: "SC",
            cidade: "Lages",
            bairro: "Maciel",
            nome: "Rua Larissa",
            cep: cep);

        if (!logradouroResult.Sucesso)
        {
            throw new Exception(
                $"Falha ao criar Logradouro: " +
                $"{string.Join(", ", logradouroResult.Notificacoes.Select(n => n.Mensagem))}");
        }

        return await _logradouroRepository.Adicionar(
            logradouroResult.Valor!);
    }

    private async Task<Aluno> CriarEInserirAlunoAsync()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync();

        var cpfResult =
            Cpf.Criar(GerarCpf());

        var telefoneResult =
            Telefone.Criar(GerarTelefone());

        var emailResult =
            Email.Criar(GerarEmail());

        var senhaResult =
            Senha.Criar("LarissaSqlServer");

        var enderecoResult =
            Endereco.Criar(
                logradouro.Cep,
                logradouro,
                "100",
                "Maciel");

        if (!cpfResult.Sucesso ||
            !telefoneResult.Sucesso ||
            !emailResult.Sucesso ||
            !senhaResult.Sucesso ||
            !enderecoResult.Sucesso)
        {
            throw new Exception(
                "Falha ao criar os dados do aluno.");
        }

        var alunoResult =
            Aluno.Criar(
                id: 0,
                nome: "Larissa Maciel",
                cpf: cpfResult.Valor!,
                dataNascimento: new DateOnly(2000, 1, 1),
                telefone: telefoneResult.Valor!,
                email: emailResult.Valor!,
                endereco: enderecoResult.Valor!,
                senha: senhaResult.Valor!,
                foto: Arquivo.Criar(
                    "foto",
                    ".jpg",
                    new byte[] { 1, 2, 3 }).Valor!);

        if (!alunoResult.Sucesso)
        {
            throw new Exception(
                $"Falha ao criar Aluno: " +
                $"{string.Join(", ", alunoResult.Notificacoes.Select(n => n.Mensagem))}");
        }

        return await _repository.Adicionar(
            alunoResult.Valor!);
    }

    [Fact]
    public async Task Aluno_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        Assert.NotNull(aluno);
        Assert.True(aluno.Id > 0);

        var obtido =
            await _repository.ObterPorId(
                aluno.Id);

        Assert.NotNull(obtido);

        Assert.Equal(
            aluno.Id,
            obtido.Id);

        Assert.Equal(
            aluno.Nome,
            obtido.Nome);

        Assert.Equal(
            aluno.Cpf.Valor,
            obtido.Cpf.Valor);

        Assert.Equal(
            aluno.Email.Valor,
            obtido.Email.Valor);
    }

    [Fact]
    public async Task Aluno_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtido =
            await _repository.ObterPorId(999999);

        Assert.Null(obtido);
    }

    [Fact]
    public async Task Aluno_ObterTodos_Sucesso()
    {
        await CriarEInserirAlunoAsync();

        var todos =
            await _repository.ObterTodos();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task Aluno_Atualizar_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var cpfResult =
            Cpf.Criar(GerarCpf());

        var telefoneResult =
            Telefone.Criar(GerarTelefone());

        var emailResult =
            Email.Criar(GerarEmail());

        var senhaResult =
            Senha.Criar("LarissaSqlServer");

        var endereco =
            aluno.Endereco;

        var atualizado =
            Aluno.Criar(
                id: aluno.Id,
                nome: "Larissa Atualizada",
                cpf: cpfResult.Valor!,
                dataNascimento: new DateOnly(2001, 2, 2),
                telefone: telefoneResult.Valor!,
                email: emailResult.Valor!,
                endereco: endereco,
                senha: senhaResult.Valor!,
                foto: aluno.Foto!);

        Assert.True(atualizado.Sucesso);
        Assert.NotNull(atualizado.Valor);

        var resultado =
            await _repository.Atualizar(
                atualizado.Valor!);

        Assert.NotNull(resultado);

        Assert.Equal(
            "Larissa Atualizada",
            resultado.Nome);

        var noBanco =
            await _repository.ObterPorId(
                aluno.Id);

        Assert.NotNull(noBanco);

        Assert.Equal(
            "Larissa Atualizada",
            noBanco.Nome);
    }

    [Fact]
    public async Task Aluno_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync();

        var cpfResult =
            Cpf.Criar(GerarCpf());

        var telefoneResult =
            Telefone.Criar(GerarTelefone());

        var emailResult =
            Email.Criar(GerarEmail());

        var senhaResult =
            Senha.Criar("LarissaSQLite");

        var enderecoResult =
            Endereco.Criar(
                logradouro.Cep,
                logradouro,
                "100",
                "Maciel");

        var aluno =
            Aluno.Criar(
                id: 999999,
                nome: "Aluno Fake",
                cpf: cpfResult.Valor!,
                dataNascimento: new DateOnly(2000, 1, 1),
                telefone: telefoneResult.Valor!,
                email: emailResult.Valor!,
                endereco: enderecoResult.Valor!,
                senha: senhaResult.Valor!,
                foto: Arquivo.Criar(
                    "foto",
                    ".jpg",
                    new byte[] { 1 }).Valor!);

        Assert.True(aluno.Sucesso);
        Assert.NotNull(aluno.Valor);

        var ex =
            await Assert.ThrowsAsync<InfrastructureException>(
                () => _repository.Atualizar(
                    aluno.Valor!));

        Assert.Equal(
            "REGISTRO_NAO_ENCONTRADO",
            ex.ErrorCode);
    }

    [Fact]
    public async Task Aluno_Remover_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var removido =
            await _repository.Remover(
                aluno.Id);

        Assert.True(removido);

        var noBanco =
            await _repository.ObterPorId(
                aluno.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Aluno_Remover_RetornaFalseQuandoInexistente()
    {
        var removido =
            await _repository.Remover(999999);

        Assert.False(removido);
    }

    [Fact]
    public async Task Aluno_ObterPorCpf_SucessoENulo()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var obtido =
            await _repository.ObterPorCpf(
                aluno.Cpf);

        Assert.NotNull(obtido);

        Assert.Equal(
            aluno.Id,
            obtido.Id);

        Assert.Equal(
            aluno.Cpf.Valor,
            obtido.Cpf.Valor);

        var cpfInexistente =
            Cpf.Criar(GerarCpf());

        Assert.True(cpfInexistente.Sucesso);

        var naoObtido =
            await _repository.ObterPorCpf(
                cpfInexistente.Valor!);

        Assert.Null(naoObtido);
    }

    [Fact]
    public async Task Aluno_ObterPorEmail_SucessoENulo()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var obtido =
            await _repository.ObterPorEmail(
                aluno.Email);

        Assert.NotNull(obtido);

        Assert.Equal(
            aluno.Id,
            obtido.Id);

        Assert.Equal(
            aluno.Email.Valor,
            obtido.Email.Valor);

        var emailInexistente =
            Email.Criar(GerarEmail());

        Assert.True(emailInexistente.Sucesso);

        var naoObtido =
            await _repository.ObterPorEmail(
                emailInexistente.Valor!);

        Assert.Null(naoObtido);
    }

    [Fact]
    public async Task Aluno_CpfJaExiste_ValidacaoCorreta()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var existe =
            await _repository.CpfJaExiste(
                aluno.Cpf);

        Assert.True(existe);

        var existeMesmoId =
            await _repository.CpfJaExiste(
                aluno.Cpf,
                aluno.Id);

        Assert.False(existeMesmoId);

        var cpfInedito =
            Cpf.Criar(GerarCpf());

        Assert.True(cpfInedito.Sucesso);

        var existeInedito =
            await _repository.CpfJaExiste(
                cpfInedito.Valor!);

        Assert.False(existeInedito);
    }

    [Fact]
    public async Task Aluno_EmailJaExiste_ValidacaoCorreta()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var existe =
            await _repository.EmailJaExiste(
                aluno.Email);

        Assert.True(existe);

        var existeMesmoId =
            await _repository.EmailJaExiste(
                aluno.Email,
                aluno.Id);

        Assert.False(existeMesmoId);

        var emailInedito =
            Email.Criar(GerarEmail());

        Assert.True(emailInedito.Sucesso);

        var existeInedito =
            await _repository.EmailJaExiste(
                emailInedito.Valor!);

        Assert.False(existeInedito);
    }
}