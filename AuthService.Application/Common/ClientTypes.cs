namespace AuthService.Application.Common
{
    public static class ClientTypes
    {
        public const string Web = "web";
        public const string Mobile = "mobile";
        public const string Desktop = "desktop";
        public static readonly IReadOnlySet<string> All = new HashSet<string>() { Web, Mobile, Desktop };
        public static bool IsValid(string value)
        {
            return All.Contains(value);
        }
    }

}
