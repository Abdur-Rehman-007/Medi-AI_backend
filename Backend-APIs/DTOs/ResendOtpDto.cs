namespace Backend_APIs.DTOs
{
    /// <summary>
    /// DTO for resend OTP request
    /// </summary>
    public class ResendOtpDto
    {
        public string Email { get; set; } = null!;
    }
}
