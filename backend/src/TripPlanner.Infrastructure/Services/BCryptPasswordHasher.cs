using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Infrastructure.Services;

/// <summary>
/// BCrypt implementation of password hashing.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
