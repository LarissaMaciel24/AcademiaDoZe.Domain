using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Larissa Maciel

using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa
{
    private Aluno(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto)
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
    }

    public static Result<Aluno> Criar(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto)
    {
        var notificacoes = new List<Notification>();

        nome = NormalizadoService.PrimeiraLetraMaiuscula(nome);

        if (string.IsNullOrWhiteSpace(nome))
            notificacoes.Add(new Notification("Nome", "O nome do aluno é obrigatório."));

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

        if (notificacoes.Any())
            return Result<Aluno>.Failure(notificacoes);

        return Result<Aluno>.Success(
            new Aluno(
                id,
                nome,
                cpf!,
                dataNascimento,
                telefone!,
                email!,
                endereco!,
                senha!,
                foto!));
    }
}