using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Auth.DTOs;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Auth.Commands.RegisterUser;

/// <summary>
/// Handler for <see cref="RegisterUserCommand"/>.
/// Creates a new user account with hashed password.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisteredUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisteredUserDto> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("Email is already registered");
        }

        // Create the user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLower(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            DisplayName = request.DisplayName,
            EmailVerified = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisteredUserDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.CreatedAt
        );
    }
}
