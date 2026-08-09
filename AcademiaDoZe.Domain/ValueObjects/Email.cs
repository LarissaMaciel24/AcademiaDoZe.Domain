using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Result<Email> Criar(string valor)
    {
        var notificacoes = new List<Notification>();

        valor = NormalizadoService.ParaMinusculo(valor);

        if (string.IsNullOrWhiteSpace(valor))
            notificacoes.Add(new Notification("Email", "O e-mail é obrigatório."));

        if (!valor.Contains("@") || !valor.Contains("."))
            notificacoes.Add(new Notification("Email", "O e-mail informado é inválido."));

        if (notificacoes.Any())
            return Result<Email>.Failure(notificacoes);

        return Result<Email>.Success(new Email(valor));
    }
}