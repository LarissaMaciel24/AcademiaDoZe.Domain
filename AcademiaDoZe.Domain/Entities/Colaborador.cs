using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa, IAggregateRoot
{
    public DateOnly DataAdmissao { get; private set; }

    public ColaboradorTipo Tipo { get; private set; }

    public ColaboradorVinculo Vinculo { get; private set; }

    private Colaborador(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto,
        DateOnly dataAdmissao,
        ColaboradorTipo tipo,
        ColaboradorVinculo vinculo)
        : base(
            id,
            nome,
            cpf,
            dataNascimento,
            telefone,
            email,
            endereco,
            senha,
            foto)
    {
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }

    public static Result<Colaborador> Criar(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto,
        DateOnly dataAdmissao,
        ColaboradorTipo tipo,
        ColaboradorVinculo vinculo)
    {
        var notificacoes = new List<Notification>();

        nome = NormalizadoService.PrimeiraLetraMaiuscula(nome);

        if (string.IsNullOrWhiteSpace(nome))
            notificacoes.Add(new Notification("Nome", "O nome é obrigatório."));

        if (cpf == null)
            notificacoes.Add(new Notification("Cpf", "O CPF é obrigatório."));

        if (telefone == null)
            notificacoes.Add(new Notification("Telefone", "O telefone é obrigatório."));

        if (email == null)
            notificacoes.Add(new Notification("Email", "O e-mail é obrigatório."));

        if (endereco == null)
            notificacoes.Add(new Notification("Endereco", "O endereço é obrigatório."));

        if (senha == null)
            notificacoes.Add(new Notification("Senha", "A senha é obrigatória."));

        if (foto == null)
            notificacoes.Add(new Notification("Foto", "A foto é obrigatória."));

        if (dataAdmissao > DateOnly.FromDateTime(DateTime.Today))
            notificacoes.Add(new Notification("DataAdmissao", "A data de admissão não pode ser futura."));

        if (notificacoes.Any())
            return Result<Colaborador>.Failure(notificacoes);

        return Result<Colaborador>.Success(
            new Colaborador(
                id,
                nome,
                cpf!,
                dataNascimento,
                telefone!,
                email!,
                endereco!,
                senha!,
                foto!,
                dataAdmissao,
                tipo,
                vinculo));
    }

    public static object Criar(int id, string nome, string cpf, DateOnly dataNascimento, string telefone, string email, Endereco endereco, string senha, Arquivo foto, DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo)
    {
        throw new NotImplementedException();
    }
}