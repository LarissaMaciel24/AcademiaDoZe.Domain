// Larissa Maciel

namespace AcademiaDoZe.Domain.Repositories;

// Um registro da catraca (PessoaTipo: 0 = Aluno, 1 = Colaborador)
public record RegistroAcesso(int PessoaTipo, int PessoaId, DateTime DataHora);

public interface IAcessoRepository
{
    Task RegistrarAsync(
        int pessoaTipo,
        int pessoaId,
        DateTime dataHora,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<DateTime>> ObterDoDiaPorPessoaAsync(
        int pessoaTipo,
        int pessoaId,
        DateOnly data,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<RegistroAcesso>> ObterDoDiaAsync(
        DateOnly data,
        CancellationToken cancellationToken = default);
}