using AcademiaDoZe.Domain.Entities;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoColaboradorRepository : IRepository<AcessoColaborador>
{
    Task<IEnumerable<AcessoColaborador>> ObterPorColaborador(
        int colaboradorId,
        DateTime? inicio = null,
        DateTime? fim = null,
        CancellationToken cancellationToken = default);

    Task<AcessoColaborador?> ObterUltimoAcesso(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<TimeSpan> ObterHorasTrabalhadasNoDia(
        int colaboradorId,
        DateOnly data,
        CancellationToken cancellationToken = default);
}