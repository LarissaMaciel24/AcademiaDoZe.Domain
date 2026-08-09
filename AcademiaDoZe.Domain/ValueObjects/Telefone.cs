using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Valor { get; }

    private Telefone(string valor)
    {
        Valor = valor;
    }

    public static Result<Telefone> Criar(string valor)
    {
        var notificacoes = new List<Notification>();

        valor = NormalizadoService.SomenteNumeros(valor);

        if (string.IsNullOrWhiteSpace(valor))
            notificacoes.Add(new Notification("Telefone", "O telefone é obrigatório."));

        if (valor.Length < 10 || valor.Length > 11)
            notificacoes.Add(new Notification("Telefone", "O telefone deve conter 10 ou 11 dígitos."));

        if (notificacoes.Any())
            return Result<Telefone>.Failure(notificacoes);

        return Result<Telefone>.Success(new Telefone(valor));
    }
}