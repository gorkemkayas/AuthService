namespace AuthService.Infrastructure.Common
{
    public static class DeviceParser
    {
        public static string Parse(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown Device";

            userAgent = userAgent.ToLower();

            // iPhone Modelleri (temel)
            if (userAgent.Contains("iphone"))
                return ParseIphone(userAgent);

            // iPad
            if (userAgent.Contains("ipad"))
                return "iPad";

            // Mac
            if (userAgent.Contains("macintosh"))
                return "MacOS Device";

            // Windows
            if (userAgent.Contains("windows nt"))
                return ParseWindows(userAgent);

            // Android cihaz ve modeli
            if (userAgent.Contains("android"))
                return ParseAndroid(userAgent);

            // Linux
            if (userAgent.Contains("linux") && !userAgent.Contains("android"))
                return "Linux PC";

            return "Unknown Device";
        }


        private static string ParseIphone(string ua)
        {
            // iPhone 14 Pro Max | 13 Pro | 12 | vs ayırmak istersen
            if (ua.Contains("iphone") && ua.Contains("pro"))
            {
                if (ua.Contains("max"))
                    return "iPhone Pro Max";

                return "iPhone Pro";
            }

            return "iPhone";
        }


        private static string ParseWindows(string ua)
        {
            if (ua.Contains("windows nt 10"))
                return "Windows 10 PC";

            if (ua.Contains("windows nt 11"))
                return "Windows 11 PC";

            return "Windows PC";
        }


        private static string ParseAndroid(string ua)
        {
            // Android cihazlar genelde şu formatta:
            // "Android 12; SM-A525F Build/SQP1.210817.001)"
            // "Android 11; Redmi Note 10)"
            // "Android 10; Pixel 4 XL)"

            // Kullanıcıya güzel bir görünüm vermek için model çıkarıyoruz:
            var model = ExtractAndroidModel(ua);

            if (!string.IsNullOrEmpty(model))
                return $"Android Device ({model})";

            // Telefon mu tablet mi?
            if (ua.Contains("mobile"))
                return "Android Phone";

            return "Android Tablet";
        }


        private static string ExtractAndroidModel(string ua)
        {
            // Klasik model pattern: ; SM-A525F
            var start = ua.IndexOf("android");
            if (start == -1) return null;

            // Split by ; and find model-like tokens
            var parts = ua.Split(';');

            foreach (var part in parts)
            {
                var trimmed = part.Trim();

                // Samsung modelleri "SM-XXXX"
                if (trimmed.StartsWith("sm-"))
                    return trimmed.ToUpper();

                // Xiaomi modelleri: "Mi 10", "Redmi Note 8"
                if (trimmed.Contains("redmi") || trimmed.Contains("mi "))
                    return Capitalize(trimmed);

                // Google Pixel
                if (trimmed.Contains("pixel"))
                    return Capitalize(trimmed);
            }

            return null;
        }


        private static string Capitalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }

}
