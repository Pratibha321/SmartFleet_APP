namespace DriverService.Models
{
    public class Driver
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime? LicenseExpiry { get; set; }
        public string Role { get; set; } = "Driver"; // Admin|Dispatcher|Driver
    }
}
