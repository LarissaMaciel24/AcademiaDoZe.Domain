using AcademiaDoZe.Domain.Entities;
using System;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Tests.Entities;

public class ColaboradorTests
{
    [Theory]
    [InlineData("Nome", "")]
    [InlineData("Nome", " ")]
    [InlineData("Nome", "   ")]
    [InlineData("Nome", "\t")]
    [InlineData("Cpf", "João")]
    [InlineData("Telefone", "João")]
    [InlineData("Email", "João")]
    [InlineData("Endereco", "João")]
    [InlineData("Senha", "João")]
    [InlineData("Foto", "João")]
    public void Criar_DeveRetornarFalha_QuandoCampoObrigatorioEstiverInvalido(
        string campo,
        string nome)
    {
        var resultado = Colaborador.Criar(
            1,
            nome,
            (AcademiaDoZe.Domain.ValueObjects.Cpf)null!,
            DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            (AcademiaDoZe.Domain.ValueObjects.Telefone)null!,
            (AcademiaDoZe.Domain.ValueObjects.Email)null!,
            (AcademiaDoZe.Domain.ValueObjects.Endereco)null!,
            (AcademiaDoZe.Domain.ValueObjects.Senha)null!,
            (AcademiaDoZe.Domain.ValueObjects.Arquivo)null!,
            DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
            default,
           default);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotEmpty(resultado.Notificacoes);

        if (campo == "Nome")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "O nome é obrigatório.");
        }

        if (campo == "Cpf")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "O CPF é obrigatório.");
        }

        if (campo == "Telefone")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "O telefone é obrigatório.");
        }

        if (campo == "Email")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "O e-mail é obrigatório.");
        }

        if (campo == "Endereco")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "O endereço é obrigatório.");
        }

        if (campo == "Senha")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "A senha é obrigatória.");
        }

        if (campo == "Foto")
        {
            Assert.Contains(
                resultado.Notificacoes,
                n => n.Mensagem == "A foto é obrigatória.");
        }
    }
}