using MongoDB.Bson;
using System.Globalization;

namespace MusicWebAppBackend.Infrastructure.Helpers
{
    public static class FunctionHelper
    {
        public static decimal ToDecimal(this string str)
        {
            // you can throw an exception or return a default value here
            if (string.IsNullOrEmpty(str))
                return 0;

            decimal d;

            // you could throw an exception or return a default value on failure
            if (!decimal.TryParse(str, out d))
                return 0;

            return d;
        }

        public static DateTime ToDateTime_ddMMyyyy(this string str)
        {
            try
            {
                return DateTime.ParseExact(str, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static string RandomStringUnique()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static string GenerateUniqueFileName(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            string uniqueFileName = $"{RandomStringUnique()}-{fileNameWithoutExtension}{extension}";
            return uniqueFileName;
        }
    }
}
