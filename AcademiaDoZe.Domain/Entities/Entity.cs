using System;

// Larissa Maciel

namespace AcademiaDoZe.Domain.Entities;

public abstract class Entity
{
    public int Id { get; protected set; }

    protected Entity(int id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity entity)
            return false;

        return Id == entity.Id &&
               GetType() == entity.GetType();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType());
    }
}