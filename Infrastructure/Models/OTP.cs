using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class OTP : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public DateTime Expire { get; set; } = DateTime.Now.AddMinutes(3);
        public string Specify { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string EmailRegister { get; set; } = string.Empty;
    }
}
