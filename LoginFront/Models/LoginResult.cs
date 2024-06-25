namespace LoginFront.Models
{
    public class LoginResult
    {
        public string? Token { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public List<string>? Errors { get; set; }
    }
}
