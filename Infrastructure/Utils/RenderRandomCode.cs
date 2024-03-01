using System.Text;
using System;

namespace MusicWebAppBackend.Infrastructure.Helpers
{
    public static class RenderRandomCode
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static int GenerateRandomSixDigitNumber()
        {
            Random random = new Random();
            return random.Next(100000, 999999); // Số ngẫu nhiên từ 100000 đến 999999
        }

        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return stringBuilder.ToString();
        }
    }
}
