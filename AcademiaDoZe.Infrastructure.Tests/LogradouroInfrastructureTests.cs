using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

// Larissa Maciel
public class LogradouroInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _repository;

    public LogradouroInfrastructureTests()
    {
        _repository = new LogradouroRepository(
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
            cidade: "MySQL",
            bairro: "Maciel",
            nome: "Larissa",
            cep: cep);

        if (!logradouroResult.Sucesso)
        {
            throw new Exception(
                $"Falha ao criar Logradouro: " +
                $"{string.Join(", ", logradouroResult.Notificacoes.Select(n => n.Mensagem))}");
        }

        return await _repository.Adicionar(
            logradouroResult.Valor!);
    }

    [Fact]
    public async Task Logradouro_Adicionar_E_ObterPorId_Sucesso()
    {
        var cep = GerarCep();

        var logradouro = Logradouro.Criar(
            id: 0,
            pais: "Brasil",
            estado: "SC",
            cidade: "MySQL",
            bairro: "Maciel",
            nome: "Larissa",
            cep: cep);

        Assert.True(logradouro.Sucesso);
        Assert.NotNull(logradouro.Valor);

        var inserido = await _repository.Adicionar(
            logradouro.Valor!);

        Assert.NotNull(inserido);
        Assert.True(inserido.Id > 0);
        Assert.Equal(cep, inserido.Cep.Valor);

        var obtido = await _repository.ObterPorId(
            inserido.Id);

        Assert.NotNull(obtido);

        Assert.Equal(
            inserido.Id,
            obtido.Id);

        Assert.Equal(
            inserido.Cep.Valor,
            obtido.Cep.Valor);

        Assert.Equal(
            inserido.Nome,
            obtido.Nome);
    }

    [Fact]
    public async Task Logradouro_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtido =
            await _repository.ObterPorId(999999);

        Assert.Null(obtido);
    }

    [Fact]
    public async Task Logradouro_ObterTodos_Sucesso()
    {
        await CriarEInserirLogradouroAsync();

        var todos =
            await _repository.ObterTodos();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task Logradouro_Atualizar_Sucesso()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync();

        var novoCep = GerarCep();

        var logradouroAtualizado =
            Logradouro.Criar(
                id: logradouro.Id,
                pais: "Brasil",
                estado: "SC",
                cidade: "Florianópolis",
                bairro: "Bairro Novo",
                nome: "Rua Nova",
                cep: novoCep);

        Assert.True(logradouroAtualizado.Sucesso);
        Assert.NotNull(logradouroAtualizado.Valor);

        var resultado =
            await _repository.Atualizar(
                logradouroAtualizado.Valor!);

        Assert.NotNull(resultado);

        Assert.Equal(
            logradouroAtualizado.Valor!.Nome,
            resultado.Nome);

        Assert.Equal(
            logradouroAtualizado.Valor.Bairro,
            resultado.Bairro);

        Assert.Equal(
            logradouroAtualizado.Valor.Cidade,
            resultado.Cidade);

        var noBanco =
            await _repository.ObterPorId(
                logradouro.Id);

        Assert.NotNull(noBanco);

        Assert.Equal(
            resultado.Nome,
            noBanco.Nome);
    }

    [Fact]
    public async Task Logradouro_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var cep = GerarCep();

        var logradouroInexistente =
            Logradouro.Criar(
                id: 999999,
                pais: "Brasil",
                estado: "SC",
                cidade: "Cidade Fake",
                bairro: "Bairro Fake",
                nome: "Rua Fake",
                cep: cep);

        Assert.True(logradouroInexistente.Sucesso);
        Assert.NotNull(logradouroInexistente.Valor);

        var ex =
            await Assert.ThrowsAsync<InfrastructureException>(
                () => _repository.Atualizar(
                    logradouroInexistente.Valor!));

        Assert.Equal(
            "REGISTRO_NAO_ENCONTRADO",
            ex.ErrorCode);
    }

    [Fact]
    public async Task Logradouro_Remover_Sucesso()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync();

        var removido =
            await _repository.Remover(
                logradouro.Id);

        Assert.True(removido);

        var noBanco =
            await _repository.ObterPorId(
                logradouro.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Logradouro_Remover_RetornaFalseQuandoInexistente()
    {
        var removida =
            await _repository.Remover(999999);

        Assert.False(removida);
    }

    [Fact]
    public async Task Logradouro_ObterPorCep_SucessoENulo()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync();

        var obtido =
            await _repository.ObterPorCep(
                logradouro.Cep);

        Assert.NotNull(obtido);

        Assert.Equal(
            logradouro.Id,
            obtido.Id);

        var cepInexistente =
            Cep.Criar("99999999");

        Assert.True(cepInexistente.Sucesso);
        Assert.NotNull(cepInexistente.Valor);

        var naoObtido =
            await _repository.ObterPorCep(
                cepInexistente.Valor!);

        Assert.Null(naoObtido);
    }

    [Fact]
    public async Task Logradouro_CepJaExiste_ValidacaoCorreta()
    {
        var logradouro =
            await CriarEInserirLogradouroAsync();

        var existe =
            await _repository.CepJaExiste(
                logradouro.Cep);

        Assert.True(existe);

        var existeMesmoId =
            await _repository.CepJaExiste(
                logradouro.Cep,
                logradouro.Id);

        Assert.False(existeMesmoId);

        var cepInedito =
            Cep.Criar(GerarCep());

        Assert.True(cepInedito.Sucesso);
        Assert.NotNull(cepInedito.Valor);

        var existeInedito =
            await _repository.CepJaExiste(
                cepInedito.Valor!);

        Assert.False(existeInedito);
    }

    [Fact]
    public async Task Logradouro_ObterPorCidade_FiltragemCorreta()
    {
        var cep = GerarCep();

        var cidadeOriginal =
            "CidadeUnica_" +
            Guid.NewGuid().ToString("N")[..5];

        var logradouro =
            Logradouro.Criar(
                id: 0,
                pais: "Brasil",
                estado: "SC",
                cidade: cidadeOriginal,
                bairro: "Bairro Y",
                nome: "Rua X",
                cep: cep);

        Assert.True(logradouro.Sucesso);
        Assert.NotNull(logradouro.Valor);

        var cidadeCriada =
            logradouro.Valor!.Cidade;

        await _repository.Adicionar(
            logradouro.Valor!);

        var resultados =
            await _repository.ObterPorCidade(
                cidadeCriada);

        Assert.NotNull(resultados);
        Assert.Single(resultados);

        Assert.Equal(
            cidadeCriada,
            resultados.First().Cidade);

        var resultadosVazio =
            await _repository.ObterPorCidade(
                "CidadeInexistente_123");

        Assert.Empty(resultadosVazio);
    }

    [Fact]
    public async Task Logradouro_ObterPorBairro_FiltragemCorreta()
    {
        var cep = GerarCep();

        var cidadeOriginal =
            "Cidade_" +
            Guid.NewGuid().ToString("N")[..5];

        var bairroOriginal =
            "Bairro_" +
            Guid.NewGuid().ToString("N")[..5];

        var logradouro =
            Logradouro.Criar(
                id: 0,
                pais: "Brasil",
                estado: "SC",
                cidade: cidadeOriginal,
                bairro: bairroOriginal,
                nome: "Rua Z",
                cep: cep);

        Assert.True(logradouro.Sucesso);
        Assert.NotNull(logradouro.Valor);

        var cidadeCriada =
            logradouro.Valor!.Cidade;

        var bairroCriado =
            logradouro.Valor.Bairro;

        await _repository.Adicionar(
            logradouro.Valor!);

        var resultados =
            await _repository.ObterPorBairro(
                cidadeCriada,
                bairroCriado);

        Assert.NotNull(resultados);
        Assert.Single(resultados);

        Assert.Equal(
            cidadeCriada,
            resultados.First().Cidade);

        Assert.Equal(
            bairroCriado,
            resultados.First().Bairro);

        var resultadosVazio =
            await _repository.ObterPorBairro(
                cidadeCriada,
                "BairroInexistente");

        Assert.Empty(resultadosVazio);
    }
}