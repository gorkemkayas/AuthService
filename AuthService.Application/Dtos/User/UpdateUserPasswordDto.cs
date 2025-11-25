namespace AuthService.Application.Dtos.User
{
    public class UpdateUserPasswordDto
    {
        public string UserId { get; set; } = null!;  // Şifre değişecek kullanıcı
        public string NewPassword { get; set; } = null!;
    }

}
