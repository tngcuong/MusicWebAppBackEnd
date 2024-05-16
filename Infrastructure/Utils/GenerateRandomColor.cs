using Microsoft.AspNetCore.Mvc;
using System;

namespace MusicWebAppBackend.Infrastructure.Utils
{
    public static class GenerateRandomColor
    {
        public static string GetRandomGradient()
        {
            string startColor = GetRandomColor();
            string endColor = GetRandomColor();
            return  $"linear-gradient(to right, {startColor}, {endColor})";
        }

        private static string GetRandomColor()
        {
            Random _random = new Random();
            int red = _random.Next(128, 256);
            int green = _random.Next(128, 256);
            int blue = _random.Next(128, 256);
            return $"rgb({red}, {green}, {blue})";
        }
    }
}
