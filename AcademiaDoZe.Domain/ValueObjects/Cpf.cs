using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Result<Cpf> Criar(string valor)
    {
        var notificacoes = new List<Notification>();

        valor = NormalizadoService.SomenteNumeros(valor);

        if (string.IsNullOrWhiteSpace(valor))
            notificacoes.Add(new Notification("Cpf", "O CPF é obrigatório."));

        if (valor.Length != 11)
            notificacoes.Add(new Notification("Cpf", "O CPF deve conter 11 dígitos."));

        if (notificacoes.Any())
            return Result<Cpf>.Failure(notificacoes);

        return Result<Cpf>.Success(new Cpf(valor));
    }
}