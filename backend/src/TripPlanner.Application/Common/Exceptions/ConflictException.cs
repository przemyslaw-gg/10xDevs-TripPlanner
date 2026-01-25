namespace TripPlanner.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when there is a conflict with the current state of a resource.
/// For example, when trying to delete a resource that is referenced by other entities.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException()
        : base()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
