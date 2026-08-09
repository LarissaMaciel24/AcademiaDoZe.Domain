using System;

// Larissa Maciel

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public abstract class Pessoa : Entity
{
    public string Nome { get; private set; }

    public Cpf Cpf { get; private set; }

    public DateOnly DataNascimento { get; private set; }

    public Telefone Telefone { get; private set; }

    public Email Email { get; private set; }

    public Endereco Endereco { get; private set; }

    public Senha Senha { get; private set; }

    public Arquivo Foto { get; private set; }

    protected Pessoa(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto)
        : base(id)
    {
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
        Telefone = telefone;
        Email = email;
        Endereco = endereco;
        Senha = senha;
        Foto = foto;
    }

    protected void AlterarFoto(Arquivo foto)
    {
        Foto = foto;
    }

    protected void AlterarEndereco(Endereco endereco)
    {
        Endereco = endereco;
    }

    protected void AlterarTelefone(Telefone telefone)
    {
        Telefone = telefone;
    }

    protected void AlterarSenha(Senha senha)
    {
        Senha = senha;
    }
}