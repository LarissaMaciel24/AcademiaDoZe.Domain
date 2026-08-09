using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string valor)
    {
        var notificacoes = new List<Notification>();

        if (string.IsNullOrWhiteSpace(valor))
            notificacoes.Add(new Notification("Senha", "A senha é obrigatória."));

        if (notificacoes.Any())
            return Result<Senha>.Failure(notificacoes);

        return Result<Senha>.Success(new Senha(valor));
    }
}