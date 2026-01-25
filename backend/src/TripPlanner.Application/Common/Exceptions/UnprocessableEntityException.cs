namespace TripPlanner.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a request is valid but cannot be processed due to business logic constraints.
/// Results in HTTP 422 Unprocessable Entity response.
/// </summary>
public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException()
        : base("The request could not be processed.")
    {
    }

    public UnprocessableEntityException(string message)
        : base(message)
    {
    }

    public UnprocessableEntityException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
