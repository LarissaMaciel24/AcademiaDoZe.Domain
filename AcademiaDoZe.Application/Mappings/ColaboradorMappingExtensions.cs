using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Commom;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

//Larissa Maciel
public static class ColaboradorMappingExtensions
{
    public static ColaboradorDto ToDto(this Colaborador colaborador)
    {
        ArgumentNullException.ThrowIfNull(colaborador);

        return new ColaboradorDto
        {
            Id = colaborador.Id,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf.Valor,
            DataNascimento = colaborador.DataNascimento,
            Telefone = colaborador.Telefone.Valor,
            Email = colaborador.Email?.Valor,
            Endereco = colaborador.Endereco?.Logradouro?.ToDto(),
            Numero = colaborador.Endereco?.Numero ?? string.Empty,
            Complemento = colaborador.Endereco?.Complemento,
            Senha = null,
            Foto = colaborador.Foto != null
                ? new ArquivoDto { Nome = colaborador.Foto.Nome, Extensao = colaborador.Foto.Extensao, Conteudo = colaborador.Foto.Conteudo }
                : null,
            DataAdmissao = colaborador.DataAdmissao,
            Tipo = colaborador.Tipo.ToApplication(),
            Vinculo = colaborador.Vinculo.ToApplication()
        };
    }

    public static Colaborador ToEntity(this ColaboradorDto colaboradorDto, Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(colaboradorDto);
        ArgumentNullException.ThrowIfNull(logradouro);

        var notificacoes = new List<Notification>();

        var cpfResult = Cpf.Criar(colaboradorDto.Cpf);
        if (!cpfResult.Sucesso) notificacoes.AddRange(cpfResult.Notificacoes);

        var telefoneResult = Telefone.Criar(colaboradorDto.Telefone);
        if (!telefoneResult.Sucesso) notificacoes.AddRange(telefoneResult.Notificacoes);

        var emailResult = Email.Criar(colaboradorDto.Email ?? string.Empty);
        if (!emailResult.Sucesso) notificacoes.AddRange(emailResult.Notificacoes);

        var senhaResult = Senha.Criar(colaboradorDto.Senha ?? string.Empty);
        if (!senhaResult.Sucesso) notificacoes.AddRange(senhaResult.Notificacoes);

        var enderecoResult = Endereco.Criar(logradouro.Cep, logradouro, colaboradorDto.Numero, colaboradorDto.Complemento ?? string.Empty);
        if (!enderecoResult.Sucesso) notificacoes.AddRange(enderecoResult.Notificacoes);

        Arquivo? foto = null;
        if (colaboradorDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(colaboradorDto.Foto.Nome, colaboradorDto.Foto.Extensao, colaboradorDto.Foto.Conteudo);
            if (fotoResult.Sucesso) foto = fotoResult.Valor;
            else notificacoes.AddRange(fotoResult.Notificacoes);
        }

        if (notificacoes.Count > 0)
        {
            throw new InvalidOperationException($"Erro de validação ao converter Colaborador: {string.Join(", ", notificacoes.Select(n => n.Mensagem))}");
        }

        var result = Colaborador.Criar(
            colaboradorDto.Id,
            colaboradorDto.Nome,
            cpfResult.Valor!,
            colaboradorDto.DataNascimento,
            telefoneResult.Valor!,
            emailResult.Valor!,
            enderecoResult.Valor!,
            senhaResult.Valor!,
            foto!,
            colaboradorDto.DataAdmissao,
            colaboradorDto.Tipo.ToDomain(),
            colaboradorDto.Vinculo.ToDomain()
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao converter Colaborador: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }

    public static Colaborador UpdateFromDto(this Colaborador colaborador, ColaboradorDto colaboradorDto, Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(colaborador);
        ArgumentNullException.ThrowIfNull(colaboradorDto);
        ArgumentNullException.ThrowIfNull(logradouro);

        var notificacoes = new List<Notification>();

        var telefoneResult = Telefone.Criar(colaboradorDto.Telefone ?? colaborador.Telefone.Valor);
        if (!telefoneResult.Sucesso) notificacoes.AddRange(telefoneResult.Notificacoes);

        var emailResult = Email.Criar(colaboradorDto.Email ?? colaborador.Email.Valor);
        if (!emailResult.Sucesso) notificacoes.AddRange(emailResult.Notificacoes);

        string senhaValor = !string.IsNullOrWhiteSpace(colaboradorDto.Senha) ? colaboradorDto.Senha : colaborador.Senha.Valor;
        var senhaResult = Senha.Criar(senhaValor);
        if (!senhaResult.Sucesso) notificacoes.AddRange(senhaResult.Notificacoes);

        var numero = colaboradorDto.Numero ?? colaborador.Endereco.Numero;
        var complemento = colaboradorDto.Complemento ?? colaborador.Endereco.Complemento;
        var enderecoResult = Endereco.Criar(logradouro.Cep, logradouro, numero, complemento);
        if (!enderecoResult.Sucesso) notificacoes.AddRange(enderecoResult.Notificacoes);

        Arquivo? foto = colaborador.Foto;
        if (colaboradorDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(colaboradorDto.Foto.Nome, colaboradorDto.Foto.Extensao, colaboradorDto.Foto.Conteudo);
            if (fotoResult.Sucesso) foto = fotoResult.Valor;
            else notificacoes.AddRange(fotoResult.Notificacoes);
        }

        if (notificacoes.Count > 0)
        {
            throw new InvalidOperationException($"Erro de validação ao atualizar Colaborador: {string.Join(", ", notificacoes.Select(n => n.Mensagem))}");
        }

        var result = Colaborador.Criar(
            colaborador.Id,
            colaboradorDto.Nome ?? colaborador.Nome,
            colaborador.Cpf,
            colaboradorDto.DataNascimento != default ? colaboradorDto.DataNascimento : colaborador.DataNascimento,
            telefoneResult.Valor!,
            emailResult.Valor!,
            enderecoResult.Valor!,
            senhaResult.Valor!,
            foto!,
            colaboradorDto.DataAdmissao != default ? colaboradorDto.DataAdmissao : colaborador.DataAdmissao,
            colaboradorDto.Tipo != default ? colaboradorDto.Tipo.ToDomain() : colaborador.Tipo,
            colaboradorDto.Vinculo != default ? colaboradorDto.Vinculo.ToDomain() : colaborador.Vinculo
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao atualizar Colaborador: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }
}