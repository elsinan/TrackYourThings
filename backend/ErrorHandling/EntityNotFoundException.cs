using System;

namespace Backend.ErrorHandling;

/// <summary>
/// Exception thrown when an entity is not found in the database.
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// Constructor for EntityNotFoundException.
    /// </summary>
    /// <param name="entityName">The name of the entity that was not found.</param>
    /// <param name="id">The ID of the entity that was not found.</param>
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found.")
    {
    }

    /// <summary>
    /// Constructor for EntityNotFoundException.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public EntityNotFoundException(string message) : base(message)
    {

    }
}