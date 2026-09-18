using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class EnderecoTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoEnderecoForValido()
    {
        var cep = Cep.Criar("88500000").Valor!;

        var logradouro = Logradouro.Criar(
            1,
            "Brasil",
            "SC",
            "Lages",
            "Centro",
            "Rua Principal").Valor!;

        var resultado = Endereco.Criar(
            cep,
            logradouro,
            "100",
            "Apto 101");

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCepForNulo()
    {
        var logradouro = Logradouro.Criar(
            1,
            "Brasil",
            "SC",
            "Lages",
            "Centro",
            "Rua Principal").Valor!;

        var resultado = Endereco.Criar(
            null!,
            logradouro,
            "100",
            "");

        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoLogradouroForNulo()
    {
        var cep = Cep.Criar("88500000").Valor!;

        var resultado = Endereco.Criar(
            cep,
            null!,
            "100",
            "");

        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoNumeroForVazio()
    {
        var cep = Cep.Criar("88500000").Valor!;

        var logradouro = Logradouro.Criar(
            1,
            "Brasil",
            "SC",
            "Lages",
            "Centro",
            "Rua Principal").Valor!;

        var resultado = Endereco.Criar(
            cep,
            logradouro,
            "",
            "");

        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveLimparEspacosDoNumeroEComplemento()
    {
        var cep = Cep.Criar("88500000").Valor!;

        var logradouro = Logradouro.Criar(
            1,
            "Brasil",
            "SC",
            "Lages",
            "Centro",
            "Rua Principal").Valor!;

        var resultado = Endereco.Criar(
            cep,
            logradouro,
            " 100 ",
            "  Apto 101  ");

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("100", resultado.Valor!.Numero);
        Assert.Equal("Apto 101", resultado.Valor.Complemento);
    }
}