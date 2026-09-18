using System.Collections.Generic;
using System.Linq;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public string Nome { get; }

    public string Extensao { get; }

    public byte[] Conteudo { get; }

    private Arquivo(
        string nome,
        string extensao,
        byte[] conteudo)
    {
        Nome = nome;
        Extensao = extensao;
        Conteudo = conteudo;
    }

    public static Result<Arquivo> Criar(
        string nome,
        string extensao,
        byte[] conteudo)
    {
        var notificacoes = new List<Notification>();

        nome = NormalizadoService.LimparEspacos(nome);
        extensao = NormalizadoService.ParaMinusculo(extensao);

        if (string.IsNullOrWhiteSpace(nome))
        {
            notificacoes.Add(
                new Notification(
                    "Nome",
                    "O nome do arquivo é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(extensao))
        {
            notificacoes.Add(
                new Notification(
                    "Extensao",
                    "A extensão do arquivo é obrigatória."));
        }

        if (conteudo == null || conteudo.Length == 0)
        {
            notificacoes.Add(
                new Notification(
                    "Conteudo",
                    "O conteúdo do arquivo é obrigatório."));
        }

        if (notificacoes.Any())
            return Result<Arquivo>.Failure(notificacoes);

        return Result<Arquivo>.Success(
            new Arquivo(
                nome,
                extensao,
                conteudo!));
    }
}