using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocationById;

/// <summary>
/// Handler for <see cref="GetLocationByIdQuery"/>.
/// Retrieves a single location by its ID and maps it to a DTO.
/// </summary>
public class GetLocationByIdQueryHandler
    : IRequestHandler<GetLocationByIdQuery, LocationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetLocationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LocationDto?> Handle(
        GetLocationByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AsNoTracking()
            .Where(l => l.Id == request.Id)
            .Select(l => new LocationDto(
                l.Id,
                l.Name,
                l.Country,
                l.Timezone,
                l.CreatedAt,
                l.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
