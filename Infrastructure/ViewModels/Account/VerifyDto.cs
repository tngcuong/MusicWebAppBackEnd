namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class VerifyDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
