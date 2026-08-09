using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Entities;

public class Logradouro
{
    public string Pais { get; private set; }

    public string Estado { get; private set; }

    public string Cidade { get; private set; }

    public string Bairro { get; private set; }

    public string Nome { get; private set; }

    private Logradouro(
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome)
    {
        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        Nome = nome;
    }

    public static Result<Logradouro> Criar(
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome)
    {
        var notificacoes = new List<Notification>();

        pais = NormalizadoService.PrimeiraLetraMaiuscula(pais);
        estado = NormalizadoService.ParaMaiusculo(estado);
        cidade = NormalizadoService.PrimeiraLetraMaiuscula(cidade);
        bairro = NormalizadoService.PrimeiraLetraMaiuscula(bairro);
        nome = NormalizadoService.PrimeiraLetraMaiuscula(nome);

        if (string.IsNullOrWhiteSpace(pais))
            notificacoes.Add(new Notification("Pais", "O país é obrigatório."));

        if (string.IsNullOrWhiteSpace(estado))
            notificacoes.Add(new Notification("Estado", "O estado é obrigatório."));

        if (string.IsNullOrWhiteSpace(cidade))
            notificacoes.Add(new Notification("Cidade", "A cidade é obrigatória."));

        if (string.IsNullOrWhiteSpace(bairro))
            notificacoes.Add(new Notification("Bairro", "O bairro é obrigatório."));

        if (string.IsNullOrWhiteSpace(nome))
            notificacoes.Add(new Notification("Nome", "O logradouro é obrigatório."));

        if (notificacoes.Any())
            return Result<Logradouro>.Failure(notificacoes);

        return Result<Logradouro>.Success(
            new Logradouro(
                pais,
                estado,
                cidade,
                bairro,
                nome));
    }
}