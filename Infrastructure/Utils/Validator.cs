using MusicWebAppBackend.Infrastructure.EnumTypes;
using System.Drawing;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MusicWebAppBackend.Infrastructure.Helpers
{
    public static class Validator
    {
        public static bool IsImage(IFormFile file)
        {
            try
            {
                if (file == null) return false;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    using (var img = Image.FromStream(memoryStream))
                    {

                        return true;
                    }
                }
            }
            catch
            {

                return false;
            }
        }
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || !int.TryParse(phoneNumber, out var fone) || fone < 330000000)
            {
                return false;
            }

            phoneNumber = phoneNumber.Trim();

            var telcoVietTelPrefix = new string[] { "096", "097", "098", "086", "032", "033", "034", "035", "036", "037", "038", "039" };
            var telcoVinalPrefix = new string[] { "088", "091", "094", "081", "082", "083", "084", "085" };
            var telcoMobilePrefix = new string[] { "090", "093", "089", "070", "076", "077", "078", "079" };
            var telcoVietnamMobilePrefix = new string[] { "092", "052", "056", "058" };
            var telcoGMobilePrefix = new string[] { "099", "059" };

            var lst = new List<string>();

            lst.AddRange(telcoVietTelPrefix);
            lst.AddRange(telcoVinalPrefix);
            lst.AddRange(telcoMobilePrefix);
            lst.AddRange(telcoVietnamMobilePrefix);
            lst.AddRange(telcoGMobilePrefix);

            var foneNumber = phoneNumber.Substring(0, 3);

            return lst.Any(x => x.Equals(foneNumber));
        }

        public static bool IsValidEmail(string mailAddress)
        {
            try
            {
                MailAddress m = new MailAddress(mailAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static InputDataTypes DetectInputDataType(string inputType)
        {
            if (IsValidEmail(inputType))
            {
                return InputDataTypes.Email;
            }

            if (IsValidPhoneNumber(inputType))
            {
                return InputDataTypes.PhoneNumber;
            }

            return InputDataTypes.Unknown;
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8;
        }

        public static bool IsValidPasswordAdvanced(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                return new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=[\]{};':""\\|,.<>?\/])").IsMatch(password);
            }
            return false;
        }

        public static bool IsMP3File(IFormFile file)
        {
            // Kiểm tra xem tệp tin có tồn tại không
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Lấy phần mở rộng của tệp tin
            var fileExtension = Path.GetExtension(file.FileName);

            // Kiểm tra xem phần mở rộng có phải là mp3 không
            if (string.Equals(fileExtension, ".mp3", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}

