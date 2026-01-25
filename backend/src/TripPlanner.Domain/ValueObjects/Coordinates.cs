namespace TripPlanner.Domain.ValueObjects;

/// <summary>
/// Value object representing geographic coordinates.
/// </summary>
public record Coordinates
{
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    public Coordinates(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Calculates the distance to another point using the Haversine formula.
    /// </summary>
    /// <returns>Distance in kilometers</returns>
    public double DistanceTo(Coordinates other)
    {
        const double earthRadiusKm = 6371.0;

        var lat1Rad = ToRadians((double)Latitude);
        var lat2Rad = ToRadians((double)other.Latitude);
        var deltaLat = ToRadians((double)(other.Latitude - Latitude));
        var deltaLon = ToRadians((double)(other.Longitude - Longitude));

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
