using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Attractions.Commands.UpdateAttraction;

/// <summary>
/// Handler for <see cref="UpdateAttractionCommand"/>.
/// Updates an existing user-generated attraction.
/// Only the owner can update their own attractions.
/// </summary>
public class UpdateAttractionCommandHandler
    : IRequestHandler<UpdateAttractionCommand, AttractionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAttractionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AttractionDto> Handle(
        UpdateAttractionCommand request,
        CancellationToken cancellationToken)
    {
        // Verify the user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to update an attraction.");
        }

        // Get the attraction with its location
        var attraction = await _context.Attractions
            .Include(a => a.Location)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attraction == null)
        {
            throw new NotFoundException("Attraction", request.Id);
        }

        // Verify the attraction is user-generated (not a system/verified attraction)
        if (attraction.IsVerified)
        {
            throw new ForbiddenAccessException("Cannot modify verified attractions.");
        }

        // Verify the user is the owner
        if (attraction.CreatedByUserId != _currentUserService.UserId.Value)
        {
            throw new ForbiddenAccessException("Cannot modify attraction created by another user.");
        }

        // Update the attraction
        attraction.Name = request.Name;
        attraction.Description = request.Description;
        attraction.Latitude = request.Latitude;
        attraction.Longitude = request.Longitude;
        attraction.EstimatedDuration = request.EstimatedDuration ?? attraction.EstimatedDuration;
        attraction.ImageUrl = request.ImageUrl;

        await _context.SaveChangesAsync(cancellationToken);

        // Return the updated attraction
        return new AttractionDto(
            attraction.Id,
            attraction.LocationId,
            new LocationSummaryDto(
                attraction.Location.Id,
                attraction.Location.Name,
                attraction.Location.Country
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
