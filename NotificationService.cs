public class NotificationService
{
    private readonly IConfiguration _config;

    public NotificationService(IConfiguration config)
    {
        _config = config;
    }

    public Task SendEmailAsync(string toEmail, string subject, string message)
    {
        // Implementation for sending email  
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string message)
    {
        // Implementation for sending SMS  
        return Task.CompletedTask;
    }
}
