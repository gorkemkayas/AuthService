namespace AuthService.Application.Common
{
    public class TokenOptions
    {
        public int AccessTokenLifetimeMinutes { get; set; }
        public int RefreshTokenLifetimeDays { get; set; }
        public int WebRefreshTokenLifetimeDays { get; set; }
        public int MobileRefreshTokenLifetimeDays { get; set; }
    }
}
