using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<Aluno?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default);

    Task<Aluno?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default);

    Task<bool> CpfJaExiste(
        Cpf cpf,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailJaExiste(
        Email email,
        int? id = null,
        CancellationToken cancellationToken = default);
}