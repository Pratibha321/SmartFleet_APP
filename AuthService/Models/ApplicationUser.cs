namespace AuthService.Models;

public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    // NOTE: For demo we store plain text password. DO NOT DO THIS IN PRODUCTION.
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Driver"; // Admin | Dispatcher | Driver
    public string FullName { get; set; } = string.Empty;
}