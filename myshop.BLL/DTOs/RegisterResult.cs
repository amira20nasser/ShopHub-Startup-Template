namespace myshop.BLL.DTOs
{
    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? EmailConfirmationToken { get; set; }
    }
}
