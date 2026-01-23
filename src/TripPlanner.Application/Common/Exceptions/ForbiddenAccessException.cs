namespace TripPlanner.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a user attempts to access a resource they don't have permission to access.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("Access denied.")
    {
    }

    public ForbiddenAccessException(string message)
        : base(message)
    {
    }

    public ForbiddenAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
