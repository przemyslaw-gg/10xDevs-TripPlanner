using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Commands.ReorderTripAttractions;

/// <summary>
/// Handler for <see cref="ReorderTripAttractionsCommand"/>.
/// Batch updates the day number and order index for multiple attractions.
/// Only the trip owner can reorder attractions.
/// </summary>
public class ReorderTripAttractionsCommandHandler : IRequestHandler<ReorderTripAttractionsCommand, ReorderResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReorderTripAttractionsCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ReorderResultDto> Handle(
        ReorderTripAttractionsCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to reorder trip attractions");
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
            throw new ForbiddenAccessException("Cannot reorder attractions in trip owned by another user");
        }

        // Get all trip attractions for this trip
        var tripAttractions = await _context.TripAttractions
            .Where(ta => ta.TripId == request.TripId)
            .ToListAsync(cancellationToken);

        // Create a lookup for quick access
        var tripAttractionLookup = tripAttractions.ToDictionary(ta => ta.AttractionId);

        // Validate that all attraction IDs in the request exist in the trip
        var requestAttractionIds = request.Attractions.Select(a => a.AttractionId).ToHashSet();
        var missingAttractions = requestAttractionIds
            .Where(id => !tripAttractionLookup.ContainsKey(id))
            .ToList();

        if (missingAttractions.Any())
        {
            throw new NotFoundException(
                "TripAttraction",
                $"Attractions not found in trip: {string.Join(", ", missingAttractions)}");
        }

        // Update each attraction's position
        var updatedCount = 0;
        foreach (var item in request.Attractions)
        {
            var tripAttraction = tripAttractionLookup[item.AttractionId];

            if (tripAttraction.DayNumber != item.DayNumber || tripAttraction.OrderIndex != item.OrderIndex)
            {
                tripAttraction.DayNumber = item.DayNumber;
                tripAttraction.OrderIndex = item.OrderIndex;
                updatedCount++;
            }
        }

        // Save all changes in a single transaction
        await _context.SaveChangesAsync(cancellationToken);

        return new ReorderResultDto(
            request.TripId,
            updatedCount,
            DateTime.UtcNow);
    }
}
