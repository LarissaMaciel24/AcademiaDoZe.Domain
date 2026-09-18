using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class ArquivoTests
{
    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoArquivoForValido()
    {
        // Arrange
        var nome = "foto";
        var extensao = ".JPG";
        var conteudo = new byte[] { 1, 2, 3 };

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Empty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoNomeForVazio()
    {
        // Arrange
        var nome = "";
        var extensao = ".jpg";
        var conteudo = new byte[] { 1, 2, 3 };

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoExtensaoForVazia()
    {
        // Arrange
        var nome = "foto";
        var extensao = "";
        var conteudo = new byte[] { 1, 2, 3 };

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoConteudoForNulo()
    {
        // Arrange
        var nome = "foto";
        var extensao = ".jpg";
        byte[] conteudo = null!;

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoConteudoEstiverVazio()
    {
        // Arrange
        var nome = "foto";
        var extensao = ".jpg";
        var conteudo = Array.Empty<byte>();

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);
    }

    [Fact]
    public void Criar_DeveRemoverEspacosDoNome()
    {
        // Arrange
        var nome = "  foto  ";
        var extensao = ".jpg";
        var conteudo = new byte[] { 1, 2, 3 };

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal("foto", resultado.Valor!.Nome);
    }

    [Fact]
    public void Criar_DeveConverterExtensaoParaMinusculo()
    {
        // Arrange
        var nome = "foto";
        var extensao = ".JPG";
        var conteudo = new byte[] { 1, 2, 3 };

        // Act
        var resultado = Arquivo.Criar(
            nome,
            extensao,
            conteudo);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(".jpg", resultado.Valor!.Extensao);
    }
}