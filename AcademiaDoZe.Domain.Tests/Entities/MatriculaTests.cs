using AcademiaDoZe.Domain.Entities;
using System;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.Entities;

public class MatriculaTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Criar_DeveRetornarFalha_QuandoObjetivoEstiverVazio(
        string objetivo)
    {
        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddMonths(1);

        var resultado = Matricula.Criar(
            1,
            null!,
            default,
            dataInicio,
            dataFinal,
            objetivo,
            default,
            "",
            null!);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);

        Assert.Contains(
            resultado.Notificacoes,
            n => n.Mensagem == "O objetivo é obrigatório.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Criar_DeveRetornarFalha_QuandoAlunoForNulo(int id)
    {
        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddMonths(1);

        var resultado = Matricula.Criar(
            id,
            null!,
            default,
            dataInicio,
            dataFinal,
            "Melhorar condicionamento",
            default,
            "",
            null!);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);

        Assert.Contains(
            resultado.Notificacoes,
            n => n.Mensagem == "O aluno é obrigatório.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Criar_DeveRetornarFalha_QuandoDataFinalForAnteriorADataInicio(
        int dias)
    {
        var dataInicio = DateOnly.FromDateTime(DateTime.Today);
        var dataFinal = dataInicio.AddDays(-dias);

        var resultado = Matricula.Criar(
            1,
            null!,
            default,
            dataInicio,
            dataFinal,
            "Melhorar condicionamento",
            default,
            "",
            null!);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);

        Assert.Contains(
            resultado.Notificacoes,
            n => n.Mensagem ==
                "A data final não pode ser anterior à data inicial.");
    }
}