using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.TripAttractions.DTOs;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.TripAttractions.Commands.AddTripAttraction;

/// <summary>
/// Handler for <see cref="AddTripAttractionCommand"/>.
/// Adds an attraction to a trip. Only the trip owner can add attractions.
/// </summary>
public class AddTripAttractionCommandHandler : IRequestHandler<AddTripAttractionCommand, TripAttractionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddTripAttractionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripAttractionDto> Handle(
        AddTripAttractionCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to add attractions to a trip");
        }

        var currentUserId = _currentUserService.UserId.Value;

        // Verify trip exists and user is owner
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

        if (trip == null)
        {
            throw new NotFoundException("Trip", request.TripId);
        }

        if (trip.OwnerId != currentUserId)
        {
            throw new ForbiddenAccessException("Cannot add attractions to trip owned by another user");
        }

        // Verify attraction exists
        var attraction = await _context.Attractions
            .FirstOrDefaultAsync(a => a.Id == request.AttractionId, cancellationToken);

        if (attraction == null)
        {
            throw new NotFoundException("Attraction", request.AttractionId);
        }

        // Check if attraction is already in the trip
        var existingAssignment = await _context.TripAttractions
            .FirstOrDefaultAsync(ta => ta.TripId == request.TripId && ta.AttractionId == request.AttractionId, cancellationToken);

        if (existingAssignment != null)
        {
            throw new ConflictException("Attraction already exists in this trip");
        }

        // Create the trip attraction
        var tripAttraction = new TripAttraction
        {
            TripId = request.TripId,
            AttractionId = request.AttractionId,
            DayNumber = request.DayNumber,
            OrderIndex = request.OrderIndex,
            PlannedStartTime = null
        };

        _context.TripAttractions.Add(tripAttraction);
        await _context.SaveChangesAsync(cancellationToken);

        return new TripAttractionDto(
            tripAttraction.Id,
            tripAttraction.TripId,
            tripAttraction.AttractionId,
            tripAttraction.DayNumber,
            tripAttraction.OrderIndex,
            tripAttraction.PlannedStartTime,
            tripAttraction.CreatedAt,
            tripAttraction.UpdatedAt == tripAttraction.CreatedAt ? null : tripAttraction.UpdatedAt);
    }
}
