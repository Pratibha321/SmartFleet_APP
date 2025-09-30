namespace TripService.Models;

public enum TripStatus { Scheduled, Assigned, Active, Completed, Cancelled }

public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TripReference { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public Guid? AssignedDriverId { get; set; }
    public string? AssignedVehicleId { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Scheduled;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}