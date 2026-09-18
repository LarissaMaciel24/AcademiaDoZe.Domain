// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity, IAggregateRoot
{
    public Colaborador Colaborador { get; private set; }

    public DateTime Entrada { get; private set; }

    public DateTime Saida { get; private set; }

    private AcessoColaborador(
        int id,
        Colaborador colaborador,
        DateTime entrada,
        DateTime saida)
        : base(id)
    {
        Colaborador = colaborador;
        Entrada = entrada;
        Saida = saida;
    }

    public static Result<AcessoColaborador> Criar(
        int id,
        Colaborador colaborador,
        DateTime entrada,
        DateTime saida)
    {
        var notificacoes = new List<Notification>();

        if (colaborador == null)
            notificacoes.Add(new Notification("Colaborador", "O colaborador é obrigatório."));

        if (entrada > saida)
            notificacoes.Add(new Notification("Saida", "A saída não pode ser anterior à entrada."));

        if (notificacoes.Any())
            return Result<AcessoColaborador>.Failure(notificacoes);

        return Result<AcessoColaborador>.Success(
            new AcessoColaborador(
                id,
                colaborador!,
                entrada,
                saida));
    }

    public void RegistrarSaida(DateTime saida)
    {
        if (saida >= Entrada)
            Saida = saida;
    }
}