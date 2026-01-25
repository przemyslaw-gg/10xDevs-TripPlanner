using MediatR;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Commands.CreateTrip;

/// <summary>
/// Command to create a new trip.
/// </summary>
/// <param name="Name">The name of the trip (max 100 characters).</param>
/// <param name="LocationId">The ID of the location for the trip (optional).</param>
/// <param name="DailyHours">Number of hours planned per day (1-24).</param>
/// <param name="MaxExtensionHours">Maximum additional hours per day (0-8).</param>
/// <param name="StartTime">Daily start time in HH:mm format.</param>
public record CreateTripCommand(
    string Name,
    Guid? LocationId,
    int DailyHours,
    int MaxExtensionHours,
    string StartTime
) : IRequest<TripDto>;
