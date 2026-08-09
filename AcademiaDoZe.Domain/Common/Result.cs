using System.Collections.Generic;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Commom;

public class Result<T>
{
    public bool Sucesso { get; }

    public T? Valor { get; }

    public IReadOnlyCollection<Notification> Notificacoes { get; }

    private Result(
        bool sucesso,
        T? valor,
        IReadOnlyCollection<Notification> notificacoes)
    {
        Sucesso = sucesso;
        Valor = valor;
        Notificacoes = notificacoes;
    }

    public static Result<T> Success(T valor)
    {
        return new Result<T>(
            true,
            valor,
            new List<Notification>());
    }

    public static Result<T> Failure(IReadOnlyCollection<Notification> notificacoes)
    {
        return new Result<T>(
            false,
            default,
            notificacoes);
    }
}