using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MusicWebAppBackend.Infrastructure.Helpers
{
    public static class PasswordHasher
    {
        private const int IterationCount = 10000;

        // Chiều dài của muối (salt) được tạo ra
        private const int SaltSize = 16;

        // Độ dài của mã băm được tạo ra
        private const int HashSize = 32;

        // Phương thức để băm mật khẩu
        public static string HashPassword(string password)
        {
            // Tạo ra muối ngẫu nhiên
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Băm mật khẩu sử dụng PBKDF2 với số vòng lặp và muối đã cho
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize);

            // Kết hợp muối và mã băm thành một chuỗi để lưu trữ trong cơ sở dữ liệu
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        // Phương thức để kiểm tra mật khẩu nhập vào có trùng khớp với mật khẩu đã băm hay không
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Chuyển đổi chuỗi băm từ cơ sở dữ liệu thành mảng byte
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Tách muối từ chuỗi băm
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Băm mật khẩu nhập vào với muối đã lưu trong cơ sở dữ liệu
            byte[] hashToCheck = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize);

            // So sánh chuỗi băm đã nhập với chuỗi băm từ cơ sở dữ liệu
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hashToCheck[i])
                {
                    return false; // Mật khẩu không trùng khớp
                }
            }

            return true; // Mật khẩu trùng khớp
        }
    }
}
