using AcademiaDoZe.Domain.Entities;
using System;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoAlunoTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void Criar_DeveRetornarFalha_QuandoAlunoForNulo(int id)
    {
        var entrada = DateTime.Today.AddHours(10);
        var saida = DateTime.Today.AddHours(11);

        var resultado = AcessoAluno.Criar(
            id,
            null!,
            entrada,
            saida);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);

        Assert.Contains(
            resultado.Notificacoes,
            n => n.Mensagem == "O aluno é obrigatório.");
    }

    [Theory]
    [InlineData(10, 9)]
    [InlineData(11, 10)]
    [InlineData(12, 11)]
    [InlineData(15, 14)]
    [InlineData(18, 17)]
    [InlineData(20, 19)]
    public void Criar_DeveRetornarFalha_QuandoEntradaForDepoisDaSaida(
        int horaEntrada,
        int horaSaida)
    {
        var entrada = DateTime.Today.AddHours(horaEntrada);
        var saida = DateTime.Today.AddHours(horaSaida);

        var resultado = AcessoAluno.Criar(
            1,
            null!,
            entrada,
            saida);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);

        Assert.Contains(
            resultado.Notificacoes,
            n => n.Mensagem == "O aluno é obrigatório.");

        Assert.Contains(
            resultado.Notificacoes,
            n => n.Mensagem == "A saída não pode ser anterior à entrada.");
    }
}