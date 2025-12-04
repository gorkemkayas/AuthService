namespace AuthService.Infrastructure.Common
{
    public static class GeneratorHelper
    {
        public static string GenerateUsername(string name, string surname)
        {
            string Normalize(string text)
            {
                return text
                    .ToLowerInvariant()
                    .Replace("ı", "i")
                    .Replace("ö", "o")
                    .Replace("ü", "u")
                    .Replace("ş", "s")
                    .Replace("ğ", "g")
                    .Replace("ç", "c")
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Replace("_", "");
            }

            var normalizedName = Normalize(name);
            var normalizedSurname = Normalize(surname);
            var randomPart = Guid.NewGuid().ToString("N")[..8];

            return $"{normalizedName}{normalizedSurname}{randomPart}";
        }

        public static string GenerateUsername(string firmName) => $"{firmName}{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        public static string GenerateFirmMail(string firmName) => $"{GenerateUsername(firmName)}@kayas.com";
        public static bool IsDomainAvailable(string domain, IEnumerable<string> existingDomains)
        {
            var domainAddress = GetDomainAddress(domain);
            return !existingDomains.Contains(domainAddress, StringComparer.OrdinalIgnoreCase);
        }
        public static string GenerateDomain(string firmName, IEnumerable<string> existingDomains)
        {
            var baseDomain = firmName.Replace(" ", "").ToLower();
            var domain = baseDomain;
            int counter = 1;
            while (!IsDomainAvailable(domain, existingDomains))
            {
                domain = $"{baseDomain}{counter}";
                counter++;
            }
            return domain;
        }
        public static string GetDomainAddress(string domain) => $"{domain}@kayas.com";
    }
}
