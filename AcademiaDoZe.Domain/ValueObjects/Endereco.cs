using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public Cep Cep { get; }

    public Logradouro Logradouro { get; }

    public string Numero { get; }

    public string Complemento { get; }

    private Endereco(
        Cep cep,
        Logradouro logradouro,
        string numero,
        string complemento)
    {
        Cep = cep;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
    }

    public static Result<Endereco> Criar(
        Cep cep,
        Logradouro logradouro,
        string numero,
        string complemento)
    {
        var notificacoes = new List<Notification>();

        numero = NormalizadoService.LimparEspacos(numero);
        complemento = NormalizadoService.LimparEspacos(complemento);

        if (cep == null)
            notificacoes.Add(new Notification("Cep", "O CEP é obrigatório."));

        if (logradouro == null)
            notificacoes.Add(new Notification("Logradouro", "O logradouro é obrigatório."));

        if (string.IsNullOrWhiteSpace(numero))
            notificacoes.Add(new Notification("Numero", "O número é obrigatório."));

        if (notificacoes.Any())
            return Result<Endereco>.Failure(notificacoes);

        return Result<Endereco>.Success(
            new Endereco(
                cep,
                logradouro,
                numero,
                complemento));
    }
}