using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class SenhaTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoSenhaForValida()
    {
        // Arrange
        var senha = "123456";

        // Act
        var resultado = Senha.Criar(senha);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("123456", resultado.Valor!.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoSenhaForVazia()
    {
        // Arrange
        var senha = "";

        // Act
        var resultado = Senha.Criar(senha);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoSenhaForApenasEspacos()
    {
        // Arrange
        var senha = "   ";

        // Act
        var resultado = Senha.Criar(senha);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }
}