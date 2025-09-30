namespace NotificationService.Models
{

    public enum NotificationType { Email = 0, Sms = 1 }
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Recipient { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Email;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
