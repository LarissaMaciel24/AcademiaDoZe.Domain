using AcademiaDoZe.Domain.Entities;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.Entities;

public class LogradouroTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoDadosForemValidos()
    {

        var resultado = Logradouro.Criar(
            1,
            "brasil",
            "sp",
            "são paulo",
            "centro",
            "rua das flores");

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Theory]
    [InlineData("sp", "SP")]
    [InlineData("Sp", "SP")]
    [InlineData("sP", "SP")]
    [InlineData("SP", "SP")]
    public void Criar_DeveConverterEstadoParaMaiusculo(
        string estado,
        string esperado)
    {
        var resultado = Logradouro.Criar(
            1,
            "brasil",
            estado,
            "são paulo",
            "centro",
            "rua das flores");

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(esperado, resultado.Valor!.Estado);
    }

    [Fact]
    public void Criar_DeveNormalizarCampos()
    {
        var resultado = Logradouro.Criar(
            1,
            "brasil",
            "sp",
            "são paulo",
            "centro",
            "rua das flores");

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);

        Assert.Equal("Brasil", resultado.Valor!.Pais);
        Assert.Equal("São Paulo", resultado.Valor.Cidade);
        Assert.Equal("Centro", resultado.Valor.Bairro);
        Assert.Equal("Rua Das Flores", resultado.Valor.Nome);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoPaisEstiverVazio()
    {
        var resultado = Logradouro.Criar(
            1,
            "",
            "SP",
            "São Paulo",
            "Centro",
            "Rua");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCamposEstiveremVazios()
    {
        var resultado = Logradouro.Criar(
            1,
            "",
            "",
            "",
            "",
            "");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);

        Assert.Equal(5, resultado.Notificacoes.Count);
    }
}