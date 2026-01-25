namespace TripPlanner.Application.Common.Interfaces;

/// <summary>
/// Service interface for accessing current authenticated user information.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
