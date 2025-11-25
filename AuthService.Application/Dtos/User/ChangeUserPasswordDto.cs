namespace AuthService.Application.Dtos.User
{
    public class ChangeUserPasswordDto
    {
        public string CurrentPassword { get; set; } = null!;  // Mevcut şifre doğrulama için
        public string NewPassword { get; set; } = null!;      // Yeni şifre
        public string? ConfirmPassword { get; set; }          // Opsiyonel, frontend doğrulama için
    }

}
