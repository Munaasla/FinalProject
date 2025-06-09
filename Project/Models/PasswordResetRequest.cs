namespace Project.Models
{
    public class PasswordResetRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; } = false;
    }
}
