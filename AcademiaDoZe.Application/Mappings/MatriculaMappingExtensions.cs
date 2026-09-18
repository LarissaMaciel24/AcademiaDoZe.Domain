using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

//Larissa Maciel
public static class MatriculaMappingExtensions
{
    public static MatriculaDto ToDto(this Matricula matricula, AlunoDto alunoDto)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(alunoDto);

        return new MatriculaDto
        {
            Id = matricula.Id,
            AlunoMatricula = alunoDto,
            Plano = matricula.Plano.ToApplication(),
            DataInicio = matricula.DataInicio,
            DataFim = matricula.DataFinal,
            Objetivo = matricula.Objetivo,
            RestricoesMedicas = matricula.Restricoes.ToApplication(),
            ObservacoesRestricoes = matricula.Observacoes,
            LaudoMedico = matricula.LaudoMedico != null
                ? new ArquivoDto { Nome = matricula.LaudoMedico.Nome, Extensao = matricula.LaudoMedico.Extensao, Conteudo = matricula.LaudoMedico.Conteudo }
                : null
        };
    }

    public static Matricula ToEntity(this MatriculaDto matriculaDto, Aluno aluno, DateOnly dataFinal)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);
        ArgumentNullException.ThrowIfNull(aluno);

        Arquivo? laudo = null;
        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult = Arquivo.Criar(matriculaDto.LaudoMedico.Nome, matriculaDto.LaudoMedico.Extensao, matriculaDto.LaudoMedico.Conteudo);
            if (laudoResult.Sucesso) laudo = laudoResult.Valor;
        }

        var result = Matricula.Criar(
            matriculaDto.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            dataFinal,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            matriculaDto.ObservacoesRestricoes ?? string.Empty,
            laudo!
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao converter Matrícula: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }

    public static Matricula UpdateFromDto(this Matricula matricula, MatriculaDto matriculaDto, Aluno aluno, DateOnly dataFinal)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(matriculaDto);
        ArgumentNullException.ThrowIfNull(aluno);

        Arquivo? laudo = matricula.LaudoMedico;
        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult = Arquivo.Criar(matriculaDto.LaudoMedico.Nome, matriculaDto.LaudoMedico.Extensao, matriculaDto.LaudoMedico.Conteudo);
            if (laudoResult.Sucesso) laudo = laudoResult.Valor;
        }

        var result = Matricula.Criar(
            matricula.Id,
            aluno,
            matriculaDto.Plano != default ? matriculaDto.Plano.ToDomain() : matricula.Plano,
            matriculaDto.DataInicio != default ? matriculaDto.DataInicio : matricula.DataInicio,
            dataFinal,
            matriculaDto.Objetivo ?? matricula.Objetivo,
            matriculaDto.RestricoesMedicas != default ? matriculaDto.RestricoesMedicas.ToDomain() : matricula.Restricoes,
            matriculaDto.ObservacoesRestricoes ?? matricula.Observacoes,
            laudo!
        );

        if (!result.Sucesso)
        {
            throw new InvalidOperationException($"Erro de validação ao atualizar Matrícula: {string.Join(", ", result.Notificacoes.Select(n => n.Mensagem))}");
        }

        return result.Valor!;
    }
}