using AcademiaDoZe.Infrastructure.Data;

[assembly: CollectionBehavior(
    CollectionBehavior.CollectionPerAssembly,
    DisableTestParallelization = true)]

namespace AcademiaDoZe.Infrastructure.Tests;

// Larissa Maciel
public abstract class TestBase
{
    private const DatabaseType SelectedDatabaseType =
    DatabaseType.SqlServer;

    protected string ConnectionString { get; }

    protected DatabaseType DatabaseType { get; }

    protected TestBase()
    {
        DatabaseType = SelectedDatabaseType;

        ConnectionString = DatabaseType switch
        {
            DatabaseType.SqlServer =>
               "Server=localhost\\SQLEXPRESS;" +
               "Database=db_academia_do_ze;" +
               "Integrated Security=True;" +
               "TrustServerCertificate=True;" +
               "Encrypt=True;",

            DatabaseType.MySql =>
                "Server=localhost;" +
                "Database=db_academia_do_ze;" +
                "User Id=root;" +
                "Password=root;",

            DatabaseType.Sqlite =>
    "Data Source=C:\\Users\\laris\\source\\repos\\AcademiaDoZe.Domain\\AcademiaDoZe.Infrastructure.Tests\\db_academia_do_ze.db;" +
    "Cache=Shared;",

            _ => throw new ArgumentOutOfRangeException(
                nameof(DatabaseType),
                DatabaseType,
                "SGBD não suportado para testes.")
        };
    }

    #region Geradores de dados aleatórios

    private static int _counter = 10000;

    protected static string GerarCep()
    {
        return (
            80000000 +
            ((int)(DateTime.UtcNow.Ticks % 8000000)) +
            Interlocked.Increment(ref _counter)
        )
        .ToString("D8")[..8];
    }

    protected static string GerarCpf()
    {
        return (
            10000000000L +
            (DateTime.UtcNow.Ticks % 8000000000L) +
            Interlocked.Increment(ref _counter)
        )
        .ToString("D11")[..11];
    }

    protected static string GerarEmail()
    {
        return $"user_{Guid.NewGuid().ToString("N")[..8]}@test.com";
    }

    protected static string GerarTelefone()
    {
        return (
            49990000000L +
            (DateTime.UtcNow.Ticks % 8000000000L) +
            Interlocked.Increment(ref _counter)
        )
        .ToString("D11")[..11];
    }

    #endregion
}