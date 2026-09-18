using System.Collections.Generic;
using System.Linq;

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Entities;

public class Logradouro : Entity, IAggregateRoot
{
    public string Pais { get; private set; }

    public string Estado { get; private set; }

    public string Cidade { get; private set; }

    public string Bairro { get; private set; }

    public string Nome { get; private set; }

    public Cep Cep { get; private set; }

    private Logradouro(
        int id,
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome,
        Cep cep)
        : base(id)
    {
        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        Nome = nome;
        Cep = cep;
    }

    // Compatibilidade com os testes antigos
    // que ainda não informam o CEP.
    public static Result<Logradouro> Criar(
        int id,
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome)
    {
        return Criar(
            id,
            pais,
            estado,
            cidade,
            bairro,
            nome,
            "88000000");
    }

    // Criação completa, incluindo o CEP.
    public static Result<Logradouro> Criar(
        int id,
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome,
        string cep)
    {
        var notificacoes = new List<Notification>();

        pais = NormalizadoService.PrimeiraLetraMaiuscula(pais);
        estado = NormalizadoService.ParaMaiusculo(estado);
        cidade = NormalizadoService.PrimeiraLetraMaiuscula(cidade);
        bairro = NormalizadoService.PrimeiraLetraMaiuscula(bairro);
        nome = NormalizadoService.PrimeiraLetraMaiuscula(nome);

        if (string.IsNullOrWhiteSpace(pais))
        {
            notificacoes.Add(
                new Notification(
                    "Pais",
                    "O país é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(estado))
        {
            notificacoes.Add(
                new Notification(
                    "Estado",
                    "O estado é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(cidade))
        {
            notificacoes.Add(
                new Notification(
                    "Cidade",
                    "A cidade é obrigatória."));
        }

        if (string.IsNullOrWhiteSpace(bairro))
        {
            notificacoes.Add(
                new Notification(
                    "Bairro",
                    "O bairro é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            notificacoes.Add(
                new Notification(
                    "Nome",
                    "O logradouro é obrigatório."));
        }

        var cepResult = Cep.Criar(cep);

        if (!cepResult.Sucesso)
        {
            notificacoes.AddRange(
                cepResult.Notificacoes);
        }

        if (notificacoes.Any())
        {
            return Result<Logradouro>.Failure(
                notificacoes);
        }

        return Result<Logradouro>.Success(
            new Logradouro(
                id,
                pais,
                estado,
                cidade,
                bairro,
                nome,
                cepResult.Valor!));
    }
}