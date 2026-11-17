using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

// Larissa Maciel
public class AcessoRepository : BaseRepository, IAcessoRepository
{
    public AcessoRepository(
        string connectionString,
        DatabaseType databaseType)
        : base(connectionString, databaseType)
    {
    }

    public async Task RegistrarAsync(
        int pessoaTipo,
        int pessoaId,
        DateTime dataHora,
        CancellationToken cancellationToken = default)
    {
        try
        {
            const string query =
                "INSERT INTO tb_acesso (pessoa_tipo, pessoa_id, data_hora) " +
                "VALUES (@PessoaTipo, @PessoaId, @DataHora)";

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@PessoaTipo", pessoaTipo, DbType.Int32);
            command.AddParameter("@PessoaId", pessoaId, DbType.Int32);
            command.AddParameter("@DataHora", dataHora, DbType.DateTime);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_REGISTRAR_ACESSO",
                $"Erro ao registrar acesso: {ex.Message}",
                ex);
        }
    }

    public async Task<IEnumerable<DateTime>> ObterDoDiaPorPessoaAsync(
        int pessoaTipo,
        int pessoaId,
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            const string query =
                """
                SELECT data_hora
                FROM tb_acesso
                WHERE pessoa_tipo = @PessoaTipo
                  AND pessoa_id = @PessoaId
                  AND data_hora >= @Inicio
                  AND data_hora < @Fim
                ORDER BY data_hora ASC, id_acesso ASC
                """;

            var inicio = data.ToDateTime(TimeOnly.MinValue);
            var fim = inicio.AddDays(1);

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@PessoaTipo", pessoaTipo, DbType.Int32);
            command.AddParameter("@PessoaId", pessoaId, DbType.Int32);
            command.AddParameter("@Inicio", inicio, DbType.DateTime);
            command.AddParameter("@Fim", fim, DbType.DateTime);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            var lista = new List<DateTime>();

            while (await reader.ReadAsync(cancellationToken))
            {
                lista.Add(reader.GetDateTimeValue("data_hora"));
            }

            return lista;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_OBTER_ACESSOS",
                $"Erro ao obter acessos da pessoa: {ex.Message}",
                ex);
        }
    }

    public async Task<IEnumerable<RegistroAcesso>> ObterDoDiaAsync(
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            const string query =
                """
                SELECT pessoa_tipo, pessoa_id, data_hora
                FROM tb_acesso
                WHERE data_hora >= @Inicio
                  AND data_hora < @Fim
                ORDER BY data_hora ASC, id_acesso ASC
                """;

            var inicio = data.ToDateTime(TimeOnly.MinValue);
            var fim = inicio.AddDays(1);

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Inicio", inicio, DbType.DateTime);
            command.AddParameter("@Fim", fim, DbType.DateTime);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            var lista = new List<RegistroAcesso>();

            while (await reader.ReadAsync(cancellationToken))
            {
                lista.Add(new RegistroAcesso(
                    reader.GetInt32Value("pessoa_tipo"),
                    reader.GetInt32Value("pessoa_id"),
                    reader.GetDateTimeValue("data_hora")));
            }

            return lista;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_OBTER_ACESSOS_DIA",
                $"Erro ao obter acessos do dia: {ex.Message}",
                ex);
        }
    }
}