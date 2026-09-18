using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

// Larissa Maciel
public class ColaboradorInfrastructureTests : TestBase
{
    private readonly ColaboradorRepository _repository;
    private readonly LogradouroRepository _logradouroRepository;

    public ColaboradorInfrastructureTests()
    {
        _repository = new ColaboradorRepository(
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

    private async Task<Colaborador> CriarEInserirColaboradorAsync()
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
                "Sala 1");

        if (!cpfResult.Sucesso ||
            !telefoneResult.Sucesso ||
            !emailResult.Sucesso ||
            !senhaResult.Sucesso ||
            !enderecoResult.Sucesso)
        {
            throw new Exception(
                "Falha ao criar os dados do colaborador.");
        }

        var colaboradorResult =
            Colaborador.Criar(
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
                    new byte[] { 1, 2, 3 }).Valor!,
                dataAdmissao: new DateOnly(2026, 1, 1),
                tipo: ColaboradorTipo.Administrador,
                vinculo: ColaboradorVinculo.CLT);

        if (!colaboradorResult.Sucesso)
        {
            throw new Exception(
                $"Falha ao criar Colaborador: " +
                $"{string.Join(", ", colaboradorResult.Notificacoes.Select(n => n.Mensagem))}");
        }

        return await _repository.Adicionar(
            colaboradorResult.Valor!);
    }

    [Fact]
    public async Task Colaborador_Adicionar_E_ObterPorId_Sucesso()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        Assert.NotNull(colaborador);
        Assert.True(colaborador.Id > 0);

        var obtido =
            await _repository.ObterPorId(
                colaborador.Id);

        Assert.NotNull(obtido);

        Assert.Equal(
            colaborador.Id,
            obtido.Id);

        Assert.Equal(
            colaborador.Nome,
            obtido.Nome);

        Assert.Equal(
            colaborador.Cpf.Valor,
            obtido.Cpf.Valor);

        Assert.Equal(
            colaborador.Email.Valor,
            obtido.Email.Valor);
    }

    [Fact]
    public async Task Colaborador_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtido =
            await _repository.ObterPorId(999999);

        Assert.Null(obtido);
    }

    [Fact]
    public async Task Colaborador_ObterTodos_Sucesso()
    {
        await CriarEInserirColaboradorAsync();

        var todos =
            await _repository.ObterTodos();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task Colaborador_Atualizar_Sucesso()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        var cpfResult =
            Cpf.Criar(GerarCpf());

        var telefoneResult =
            Telefone.Criar(GerarTelefone());

        var emailResult =
            Email.Criar(GerarEmail());

        var senhaResult =
            Senha.Criar("LarissaSqlServer");

        var endereco =
            colaborador.Endereco;

        var atualizado =
            Colaborador.Criar(
                id: colaborador.Id,
                nome: "Larissa Atualizada",
                cpf: cpfResult.Valor!,
                dataNascimento: new DateOnly(2001, 2, 2),
                telefone: telefoneResult.Valor!,
                email: emailResult.Valor!,
                endereco: endereco,
                senha: senhaResult.Valor!,
                foto: colaborador.Foto!,
                dataAdmissao: colaborador.DataAdmissao,
                tipo: colaborador.Tipo,
                vinculo: colaborador.Vinculo);

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
                colaborador.Id);

        Assert.NotNull(noBanco);

        Assert.Equal(
            "Larissa Atualizada",
            noBanco.Nome);
    }

    [Fact]
    public async Task Colaborador_Atualizar_LancaExcecaoQuandoInexistente()
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
            Senha.Criar("123456");

        var enderecoResult =
            Endereco.Criar(
                logradouro.Cep,
                logradouro,
                "100",
                "");

        var colaborador =
            Colaborador.Criar(
                id: 999999,
                nome: "Colaborador Fake",
                cpf: cpfResult.Valor!,
                dataNascimento: new DateOnly(2000, 1, 1),
                telefone: telefoneResult.Valor!,
                email: emailResult.Valor!,
                endereco: enderecoResult.Valor!,
                senha: senhaResult.Valor!,
                foto: Arquivo.Criar(
                    "foto",
                    ".jpg",
                    new byte[] { 1 }).Valor!,
                dataAdmissao: new DateOnly(2026, 1, 1),
                tipo: ColaboradorTipo.Administrador,
                vinculo: ColaboradorVinculo.CLT);

        Assert.True(colaborador.Sucesso);
        Assert.NotNull(colaborador.Valor);

        var ex =
            await Assert.ThrowsAsync<InfrastructureException>(
                () => _repository.Atualizar(
                    colaborador.Valor!));

        Assert.Equal(
            "REGISTRO_NAO_ENCONTRADO",
            ex.ErrorCode);
    }

    [Fact]
    public async Task Colaborador_Remover_Sucesso()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        var removido =
            await _repository.Remover(
                colaborador.Id);

        Assert.True(removido);

        var noBanco =
            await _repository.ObterPorId(
                colaborador.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Colaborador_Remover_RetornaFalseQuandoInexistente()
    {
        var removido =
            await _repository.Remover(999999);

        Assert.False(removido);
    }

    [Fact]
    public async Task Colaborador_ObterPorCpf_SucessoENulo()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        var obtido =
            await _repository.ObterPorCpf(
                colaborador.Cpf);

        Assert.NotNull(obtido);

        Assert.Equal(
            colaborador.Id,
            obtido.Id);

        Assert.Equal(
            colaborador.Cpf.Valor,
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
    public async Task Colaborador_ObterPorEmail_SucessoENulo()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        var obtido =
            await _repository.ObterPorEmail(
                colaborador.Email);

        Assert.NotNull(obtido);

        Assert.Equal(
            colaborador.Id,
            obtido.Id);

        Assert.Equal(
            colaborador.Email.Valor,
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
    public async Task Colaborador_CpfJaExiste_ValidacaoCorreta()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        var existe =
            await _repository.CpfJaExiste(
                colaborador.Cpf);

        Assert.True(existe);

        var existeMesmoId =
            await _repository.CpfJaExiste(
                colaborador.Cpf,
                colaborador.Id);

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
    public async Task Colaborador_EmailJaExiste_ValidacaoCorreta()
    {
        var colaborador =
            await CriarEInserirColaboradorAsync();

        var existe =
            await _repository.EmailJaExiste(
                colaborador.Email);

        Assert.True(existe);

        var existeMesmoId =
            await _repository.EmailJaExiste(
                colaborador.Email,
                colaborador.Id);

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