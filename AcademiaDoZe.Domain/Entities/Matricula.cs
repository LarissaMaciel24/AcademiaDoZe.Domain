using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity
{
    public Aluno Aluno { get; private set; }

    public MatriculaPlano Plano { get; private set; }

    public DateOnly DataInicio { get; private set; }

    public DateOnly DataFinal { get; private set; }

    public string Objetivo { get; private set; }

    public MatriculaRestricoes Restricoes { get; private set; }

    public string Observacoes { get; private set; }

    public Arquivo LaudoMedico { get; private set; }

    private Matricula(
        int id,
        Aluno aluno,
        MatriculaPlano plano,
        DateOnly dataInicio,
        DateOnly dataFinal,
        string objetivo,
        MatriculaRestricoes restricoes,
        string observacoes,
        Arquivo laudoMedico)
        : base(id)
    {
        Aluno = aluno;
        Plano = plano;
        DataInicio = dataInicio;
        DataFinal = dataFinal;
        Objetivo = objetivo;
        Restricoes = restricoes;
        Observacoes = observacoes;
        LaudoMedico = laudoMedico;
    }

    public static Result<Matricula> Criar(
        int id,
        Aluno aluno,
        MatriculaPlano plano,
        DateOnly dataInicio,
        DateOnly dataFinal,
        string objetivo,
        MatriculaRestricoes restricoes,
        string observacoes,
        Arquivo laudoMedico)
    {
        var notificacoes = new List<Notification>();

        objetivo = NormalizadoService.LimparEspacos(objetivo);
        observacoes = NormalizadoService.LimparEspacos(observacoes);

        if (aluno == null)
            notificacoes.Add(new Notification("Aluno", "O aluno é obrigatório."));

        if (string.IsNullOrWhiteSpace(objetivo))
            notificacoes.Add(new Notification("Objetivo", "O objetivo é obrigatório."));

        if (dataFinal < dataInicio)
            notificacoes.Add(new Notification("DataFinal", "A data final não pode ser anterior à data inicial."));

        if (notificacoes.Any())
            return Result<Matricula>.Failure(notificacoes);

        return Result<Matricula>.Success(
            new Matricula(
                id,
                aluno!,
                plano,
                dataInicio,
                dataFinal,
                objetivo,
                restricoes,
                observacoes,
                laudoMedico));
    }

    public void AlterarObservacoes(string observacoes)
    {
        Observacoes = NormalizadoService.LimparEspacos(observacoes);
    }

    public void AlterarObjetivo(string objetivo)
    {
        Objetivo = NormalizadoService.LimparEspacos(objetivo);
    }
}