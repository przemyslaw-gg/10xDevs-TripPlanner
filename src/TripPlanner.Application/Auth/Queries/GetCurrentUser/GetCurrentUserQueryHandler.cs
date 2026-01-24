using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Auth.DTOs;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Handler for <see cref="GetCurrentUserQuery"/>.
/// Returns the current authenticated user's information.
/// </summary>
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CurrentUserDto> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required");
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId.Value, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", _currentUserService.UserId.Value);
        }

        return new CurrentUserDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.EmailVerified,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
