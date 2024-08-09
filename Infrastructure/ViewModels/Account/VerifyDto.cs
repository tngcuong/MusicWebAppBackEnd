namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class VerifyDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public string OtpId { get; set; } = string.Empty;
    }
}
