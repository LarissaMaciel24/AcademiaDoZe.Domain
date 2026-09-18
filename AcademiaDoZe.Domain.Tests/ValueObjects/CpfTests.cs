using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class CpfTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoCpfForValido()
    {
        // Arrange
        var cpf = "12345678901";

        // Act
        var resultado = Cpf.Criar(cpf);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("12345678901", resultado.Valor!.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCpfForVazio()
    {
        // Arrange
        var cpf = "";

        // Act
        var resultado = Cpf.Criar(cpf);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCpfTiverMenosDeOnzeDigitos()
    {
        // Arrange
        var cpf = "1234567890";

        // Act
        var resultado = Cpf.Criar(cpf);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoCpfTiverMaisDeOnzeDigitos()
    {
        // Arrange
        var cpf = "123456789012";

        // Act
        var resultado = Cpf.Criar(cpf);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveAceitarCpfComPontuacao()
    {
        // Arrange
        var cpf = "123.456.789-01";

        // Act
        var resultado = Cpf.Criar(cpf);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("12345678901", resultado.Valor!.Valor);
    }
}