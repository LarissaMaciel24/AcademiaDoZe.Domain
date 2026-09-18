using AcademiaDoZe.Domain.Services;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.Services;

public class NormalizadoServiceTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData(" texto ", "texto")]
    [InlineData("  João da Silva  ", "João da Silva")]
    public void LimparEspacos_DeveRemoverEspacosDasExtremidades(
        string entrada,
        string esperado)
    {
        var resultado = NormalizadoService.LimparEspacos(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("joao", "Joao")]
    [InlineData("JOAO DA SILVA", "Joao Da Silva")]
    public void PrimeiraLetraMaiuscula_DeveNormalizarTexto(
        string entrada,
        string esperado)
    {
        var resultado = NormalizadoService.PrimeiraLetraMaiuscula(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("sp", "SP")]
    [InlineData(" sp ", "SP")]
    public void ParaMaiusculo_DeveConverterTextoParaMaiusculo(
        string entrada,
        string esperado)
    {
        var resultado = NormalizadoService.ParaMaiusculo(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("123456", "123456")]
    [InlineData("(49) 99999-8888", "49999998888")]
    public void SomenteNumeros_DeveManterApenasNumeros(
        string entrada,
        string esperado)
    {
        var resultado = NormalizadoService.SomenteNumeros(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("EMAIL@TESTE.COM", "email@teste.com")]
    [InlineData("  JOAO@EMAIL.COM  ", "joao@email.com")]
    public void ParaMinusculo_DeveConverterTextoParaMinusculo(
        string entrada,
        string esperado)
    {
        var resultado = NormalizadoService.ParaMinusculo(entrada);

        Assert.Equal(esperado, resultado);
    }
}