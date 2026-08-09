using System;
using System.Globalization;
using System.Text;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Services;

public static class NormalizadoService
{
    public static string LimparEspacos(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        return texto.Trim();
    }

    public static string PrimeiraLetraMaiuscula(string texto)
    {
        texto = LimparEspacos(texto);

        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower());
    }

    public static string ParaMaiusculo(string texto)
    {
        texto = LimparEspacos(texto);

        return texto.ToUpperInvariant();
    }

    public static string SomenteNumeros(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        var resultado = new StringBuilder();

        foreach (char caractere in texto)
        {
            if (char.IsDigit(caractere))
                resultado.Append(caractere);
        }

        return resultado.ToString();
    }

    public static string ParaMinusculo(string texto)
    {
        texto = LimparEspacos(texto);

        return texto.ToLowerInvariant();
    }
}