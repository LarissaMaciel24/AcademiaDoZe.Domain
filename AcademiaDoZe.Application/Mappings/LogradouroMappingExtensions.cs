using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Mappings;

//Larissa Maciel
public static class LogradouroMappingExtensions
{
    public static LogradouroDto ToDto(this Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(logradouro);

        return new LogradouroDto
        {
            Id = logradouro.Id,
            Cep = logradouro.Cep.Valor,
            Nome = logradouro.Nome,
            Bairro = logradouro.Bairro,
            Cidade = logradouro.Cidade,
            Estado = logradouro.Estado,
            Pais = logradouro.Pais
        };
    }

    public static Logradouro ToEntity(this LogradouroDto logradouroDto)
    {
        ArgumentNullException.ThrowIfNull(logradouroDto);

        var result = Logradouro.Criar(
            logradouroDto.Id,
            logradouroDto.Pais,
            logradouroDto.Estado,
            logradouroDto.Cidade,
            logradouroDto.Bairro,
            logradouroDto.Nome,
            logradouroDto.Cep
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao converter Logradouro: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }

    public static Logradouro UpdateFromDto(this Logradouro logradouro, LogradouroDto logradouroDto)
    {
        ArgumentNullException.ThrowIfNull(logradouro);
        ArgumentNullException.ThrowIfNull(logradouroDto);

        var result = Logradouro.Criar(
            logradouro.Id,
            logradouroDto.Pais ?? logradouro.Pais,
            logradouroDto.Estado ?? logradouro.Estado,
            logradouroDto.Cidade ?? logradouro.Cidade,
            logradouroDto.Bairro ?? logradouro.Bairro,
            logradouroDto.Nome ?? logradouro.Nome,
            logradouroDto.Cep ?? logradouro.Cep.Valor
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao atualizar Logradouro: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }
}