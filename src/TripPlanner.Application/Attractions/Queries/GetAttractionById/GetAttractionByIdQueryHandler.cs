using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Attractions.Queries.GetAttractionById;

/// <summary>
/// Handler for <see cref="GetAttractionByIdQuery"/>.
/// Retrieves a single attraction by its ID with related location information.
/// </summary>
public class GetAttractionByIdQueryHandler
    : IRequestHandler<GetAttractionByIdQuery, AttractionDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAttractionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttractionDto?> Handle(
        GetAttractionByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Attractions
            .AsNoTracking()
            .Include(a => a.Location)
            .Where(a => a.Id == request.Id)
            .Select(a => new AttractionDto(
                a.Id,
                a.LocationId,
                new LocationSummaryDto(
                    a.Location.Id,
                    a.Location.Name,
                    a.Location.Country
                ),
                a.Name,
                a.Description,
                a.Latitude,
                a.Longitude,
                a.Rating,
                a.ReviewCount,
                a.EstimatedDuration ?? 60,
                a.ImageUrl,
                a.IsVerified,
                a.CreatedByUserId,
                a.CreatedAt,
                a.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
