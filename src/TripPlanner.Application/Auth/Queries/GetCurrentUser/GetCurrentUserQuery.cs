using MediatR;
using TripPlanner.Application.Auth.DTOs;

namespace TripPlanner.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Query to get the current authenticated user's information.
/// </summary>
public record GetCurrentUserQuery : IRequest<CurrentUserDto>;
