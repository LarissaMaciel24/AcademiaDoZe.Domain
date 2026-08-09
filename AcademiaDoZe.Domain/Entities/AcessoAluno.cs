using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; private set; }

    public DateTime Entrada { get; private set; }

    public DateTime Saida { get; private set; }

    private AcessoAluno(
        int id,
        Aluno aluno,
        DateTime entrada,
        DateTime saida)
        : base(id)
    {
        Aluno = aluno;
        Entrada = entrada;
        Saida = saida;
    }

    public static Result<AcessoAluno> Criar(
        int id,
        Aluno aluno,
        DateTime entrada,
        DateTime saida)
    {
        var notificacoes = new List<Notification>();

        if (aluno == null)
            notificacoes.Add(new Notification("Aluno", "O aluno é obrigatório."));

        if (entrada > saida)
            notificacoes.Add(new Notification("Saida", "A saída não pode ser anterior à entrada."));

        if (notificacoes.Any())
            return Result<AcessoAluno>.Failure(notificacoes);

        return Result<AcessoAluno>.Success(
            new AcessoAluno(
                id,
                aluno!,
                entrada,
                saida));
    }

    public void RegistrarSaida(DateTime saida)
    {
        if (saida >= Entrada)
            Saida = saida;
    }
}