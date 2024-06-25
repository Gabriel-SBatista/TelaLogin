namespace LoginFront.Models
{
    public class RegisterResult
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public bool? EmailConfirmed { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
