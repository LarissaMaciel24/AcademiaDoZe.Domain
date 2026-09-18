using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoEmailForValido()
    {
        // Arrange
        var email = "larissa@email.com";

        // Act
        var resultado = Email.Criar(email);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("larissa@email.com", resultado.Valor!.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoEmailForVazio()
    {
        // Arrange
        var email = "";

        // Act
        var resultado = Email.Criar(email);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoEmailNaoPossuirArroba()
    {
        // Arrange
        var email = "larissaemail.com";

        // Act
        var resultado = Email.Criar(email);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoEmailNaoPossuirPonto()
    {
        // Arrange
        var email = "larissa@emailcom";

        // Act
        var resultado = Email.Criar(email);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveConverterEmailParaMinusculo()
    {
        // Arrange
        var email = "LARISSA@EMAIL.COM";

        // Act
        var resultado = Email.Criar(email);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("larissa@email.com", resultado.Valor!.Valor);
    }
}