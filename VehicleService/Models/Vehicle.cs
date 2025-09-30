namespace VehicleService.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        public string RegistrationNumber { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public DateTime? LastMaintenance { get; set; }

        public bool IsActive { get; set; }
    }
}
