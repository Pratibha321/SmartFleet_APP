namespace TelemetryService.Models
{
    public class TelemetryRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string VehicleId { get; set; } = string.Empty;
        public double Speed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double FuelLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
