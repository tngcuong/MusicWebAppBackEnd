namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string OtpId { get; set; } = string.Empty;
    }
}
