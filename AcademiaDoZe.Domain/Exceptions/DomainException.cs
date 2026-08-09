using System;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string mensagem)
        : base(mensagem)
    {
    }
}