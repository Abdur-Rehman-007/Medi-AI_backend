namespace Backend_APIs.DTOs
{
    /// <summary>
    /// DTO for reset password request
    /// </summary>
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
