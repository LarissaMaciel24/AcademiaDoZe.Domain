using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class TelefoneTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoTelefoneTiverDezDigitos()
    {
        // Arrange
        var telefone = "4933334444";

        // Act
        var resultado = Telefone.Criar(telefone);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("4933334444", resultado.Valor!.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoTelefoneTiverOnzeDigitos()
    {
        // Arrange
        var telefone = "49999998888";

        // Act
        var resultado = Telefone.Criar(telefone);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("49999998888", resultado.Valor!.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoTelefoneForVazio()
    {
        // Arrange
        var telefone = "";

        // Act
        var resultado = Telefone.Criar(telefone);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoTelefoneTiverMenosDeDezDigitos()
    {
        // Arrange
        var telefone = "493333444";

        // Act
        var resultado = Telefone.Criar(telefone);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoTelefoneTiverMaisDeOnzeDigitos()
    {
        // Arrange
        var telefone = "499999988888";

        // Act
        var resultado = Telefone.Criar(telefone);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRemoverCaracteresNaoNumericos()
    {
        // Arrange
        var telefone = "(49) 99999-8888";

        // Act
        var resultado = Telefone.Criar(telefone);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("49999998888", resultado.Valor!.Valor);
    }
}