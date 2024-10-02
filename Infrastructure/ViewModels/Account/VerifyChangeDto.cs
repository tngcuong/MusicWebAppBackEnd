namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class VerifyChangeDto
    {
        public string Email { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public string OtpId { get; set; } = string.Empty;
    }
}
