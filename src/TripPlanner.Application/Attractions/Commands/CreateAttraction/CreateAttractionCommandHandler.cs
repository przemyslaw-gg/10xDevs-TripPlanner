using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Attractions.Commands.CreateAttraction;

/// <summary>
/// Handler for <see cref="CreateAttractionCommand"/>.
/// Creates a new user-generated attraction and returns its details.
/// </summary>
public class CreateAttractionCommandHandler
    : IRequestHandler<CreateAttractionCommand, AttractionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateAttractionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AttractionDto> Handle(
        CreateAttractionCommand request,
        CancellationToken cancellationToken)
    {
        // Verify the user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to create an attraction.");
        }

        // Verify the location exists
        var location = await _context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.LocationId, cancellationToken);

        if (location == null)
        {
            throw new NotFoundException("Location", request.LocationId);
        }

        // Create the attraction entity
        var attraction = new Attraction
        {
            Id = Guid.NewGuid(),
            LocationId = request.LocationId,
            Name = request.Name,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            EstimatedDuration = request.EstimatedDuration ?? 60,
            ImageUrl = request.ImageUrl,
            Rating = null,
            ReviewCount = null,
            IsVerified = false, // User-generated attractions are not verified
            CreatedByUserId = _currentUserService.UserId.Value
        };

        _context.Attractions.Add(attraction);
        await _context.SaveChangesAsync(cancellationToken);

        // Return the created attraction with location details
        return new AttractionDto(
            attraction.Id,
            attraction.LocationId,
            new LocationSummaryDto(
                location.Id,
                location.Name,
                location.Country
            ),
            attraction.Name,
            attraction.Description,
            attraction.Latitude,
            attraction.Longitude,
            attraction.Rating,
            attraction.ReviewCount,
            attraction.EstimatedDuration ?? 60,
            attraction.ImageUrl,
            attraction.IsVerified,
            attraction.CreatedByUserId,
            attraction.CreatedAt,
            attraction.UpdatedAt
        );
    }
}
