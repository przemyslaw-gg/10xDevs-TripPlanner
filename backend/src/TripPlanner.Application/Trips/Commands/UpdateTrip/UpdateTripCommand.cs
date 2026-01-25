using MediatR;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Commands.UpdateTrip;

/// <summary>
/// Command to update an existing trip.
/// </summary>
/// <param name="Id">The unique identifier of the trip to update.</param>
/// <param name="Name">The new name of the trip (max 100 characters).</param>
/// <param name="LocationId">The new location ID for the trip (optional).</param>
/// <param name="DailyHours">New number of hours planned per day (1-24).</param>
/// <param name="MaxExtensionHours">New maximum additional hours per day (0-8).</param>
/// <param name="StartTime">New daily start time in HH:mm format.</param>
public record UpdateTripCommand(
    Guid Id,
    string Name,
    Guid? LocationId,
    int DailyHours,
    int MaxExtensionHours,
    string StartTime
) : IRequest<TripDto>;
