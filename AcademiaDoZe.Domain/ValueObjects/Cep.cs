using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; }

    private Cep(string valor)
    {
        Valor = valor;
    }

    public static Result<Cep> Criar(string valor)
    {
        var notificacoes = new List<Notification>();

        valor = NormalizadoService.SomenteNumeros(valor);

        if (string.IsNullOrWhiteSpace(valor))
            notificacoes.Add(new Notification("Cep", "O CEP é obrigatório."));

        if (valor.Length != 8)
            notificacoes.Add(new Notification("Cep", "O CEP deve conter 8 dígitos."));

        if (notificacoes.Any())
            return Result<Cep>.Failure(notificacoes);

        return Result<Cep>.Success(new Cep(valor));
    }
}
