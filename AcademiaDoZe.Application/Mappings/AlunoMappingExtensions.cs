using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

//Larissa Maciel
public static class AlunoMappingExtensions
{
    public static AlunoDto ToDto(this Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(aluno);

        return new AlunoDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf.Valor,
            DataNascimento = aluno.DataNascimento,
            Telefone = aluno.Telefone.Valor,
            Email = aluno.Email?.Valor,
            Endereco = aluno.Endereco?.Logradouro?.ToDto(),
            Numero = aluno.Endereco?.Numero ?? string.Empty,
            Complemento = aluno.Endereco?.Complemento,
            Senha = null,
            Foto = aluno.Foto != null
                ? new ArquivoDto { Nome = aluno.Foto.Nome, Extensao = aluno.Foto.Extensao, Conteudo = aluno.Foto.Conteudo }
                : null
        };
    }

    public static Aluno ToEntity(this AlunoDto alunoDto, Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(alunoDto);
        ArgumentNullException.ThrowIfNull(logradouro);

        var notificacoes = new List<Notification>();

        var cpfResult = Cpf.Criar(alunoDto.Cpf);
        if (!cpfResult.Sucesso) notificacoes.AddRange(cpfResult.Notificacoes);

        var telefoneResult = Telefone.Criar(alunoDto.Telefone);
        if (!telefoneResult.Sucesso) notificacoes.AddRange(telefoneResult.Notificacoes);

        var emailResult = Email.Criar(alunoDto.Email ?? string.Empty);
        if (!emailResult.Sucesso) notificacoes.AddRange(emailResult.Notificacoes);

        var senhaResult = Senha.Criar(alunoDto.Senha ?? string.Empty);
        if (!senhaResult.Sucesso) notificacoes.AddRange(senhaResult.Notificacoes);

        var enderecoResult = Endereco.Criar(logradouro.Cep, logradouro, alunoDto.Numero, alunoDto.Complemento ?? string.Empty);
        if (!enderecoResult.Sucesso) notificacoes.AddRange(enderecoResult.Notificacoes);

        Arquivo? foto = null;
        if (alunoDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(alunoDto.Foto.Nome, alunoDto.Foto.Extensao, alunoDto.Foto.Conteudo);
            if (fotoResult.Sucesso) foto = fotoResult.Valor;
            else notificacoes.AddRange(fotoResult.Notificacoes);
        }

        if (notificacoes.Count > 0)
        {
            throw new InvalidOperationException($"Erro de validação ao converter Aluno: {string.Join(", ", notificacoes.Select(n => n.Mensagem))}");
        }

        var result = Aluno.Criar(
            alunoDto.Id,
            alunoDto.Nome,
            cpfResult.Valor!,
            alunoDto.DataNascimento,
            telefoneResult.Valor!,
            emailResult.Valor!,
            enderecoResult.Valor!,
            senhaResult.Valor!,
            foto!
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao converter Aluno: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }

    public static Aluno UpdateFromDto(this Aluno aluno, AlunoDto alunoDto, Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(aluno);
        ArgumentNullException.ThrowIfNull(alunoDto);
        ArgumentNullException.ThrowIfNull(logradouro);

        var notificacoes = new List<Notification>();

        var telefoneResult = Telefone.Criar(alunoDto.Telefone ?? aluno.Telefone.Valor);
        if (!telefoneResult.Sucesso) notificacoes.AddRange(telefoneResult.Notificacoes);

        var emailResult = Email.Criar(alunoDto.Email ?? aluno.Email.Valor);
        if (!emailResult.Sucesso) notificacoes.AddRange(emailResult.Notificacoes);

        string senhaValor = !string.IsNullOrWhiteSpace(alunoDto.Senha) ? alunoDto.Senha : aluno.Senha.Valor;
        var senhaResult = Senha.Criar(senhaValor);
        if (!senhaResult.Sucesso) notificacoes.AddRange(senhaResult.Notificacoes);

        var numero = alunoDto.Numero ?? aluno.Endereco.Numero;
        var complemento = alunoDto.Complemento ?? aluno.Endereco.Complemento;
        var enderecoResult = Endereco.Criar(logradouro.Cep, logradouro, numero, complemento);
        if (!enderecoResult.Sucesso) notificacoes.AddRange(enderecoResult.Notificacoes);

        Arquivo? foto = aluno.Foto;
        if (alunoDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(alunoDto.Foto.Nome, alunoDto.Foto.Extensao, alunoDto.Foto.Conteudo);
            if (fotoResult.Sucesso) foto = fotoResult.Valor;
            else notificacoes.AddRange(fotoResult.Notificacoes);
        }

        if (notificacoes.Count > 0)
        {
            throw new InvalidOperationException($"Erro de validação ao atualizar Aluno: {string.Join(", ", notificacoes.Select(n => n.Mensagem))}");
        }

        var result = Aluno.Criar(
            aluno.Id,
            alunoDto.Nome ?? aluno.Nome,
            aluno.Cpf,
            alunoDto.DataNascimento != default ? alunoDto.DataNascimento : aluno.DataNascimento,
            telefoneResult.Valor!,
            emailResult.Valor!,
            enderecoResult.Valor!,
            senhaResult.Valor!,
            foto!
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao atualizar Aluno: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }
}