namespace AuthService.Application.Dtos.Refresh
{
    public class RefreshResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
