using AcademiaDoZe.Domain.Entities;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoAlunoRepository : IRepository<AcessoAluno>
{
    Task<IEnumerable<AcessoAluno>> ObterPorAluno(
        int alunoId,
        DateTime? inicio = null,
        DateTime? fim = null,
        CancellationToken cancellationToken = default);

    Task<AcessoAluno?> ObterUltimoAcesso(
        int alunoId,
        CancellationToken cancellationToken = default);

    Task<TimeSpan> ObterHorasFrequentadasNoDia(
        int alunoId,
        DateOnly data,
        CancellationToken cancellationToken = default);
}