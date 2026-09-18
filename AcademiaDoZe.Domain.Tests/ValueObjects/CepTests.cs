using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class CepTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoCepForValido()
    {
        // Arrange
        var cep = "88500000";

        // Act
        var resultado = Cep.Criar(cep);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("88500000", resultado.Valor!.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCepForVazio()
    {
        // Arrange
        var cep = "";

        // Act
        var resultado = Cep.Criar(cep);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCepTiverMenosDeOitoDigitos()
    {
        // Arrange
        var cep = "1234567";

        // Act
        var resultado = Cep.Criar(cep);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCepTiverMaisDeOitoDigitos()
    {
        // Arrange
        var cep = "123456789";

        // Act
        var resultado = Cep.Criar(cep);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveAceitarCepComMascara()
    {
        // Arrange
        var cep = "88500-000";

        // Act
        var resultado = Cep.Criar(cep);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("88500000", resultado.Valor!.Valor);
    }
}