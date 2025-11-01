using AuthService.Domain.Entities;
using AutoMapper;

namespace AuthService.Application.Mapping.Profiles
{
    public class RefreshTokenProfile : Profile
    {
        public RefreshTokenProfile()
        {
            CreateMap<RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>()
                .ReverseMap();
        }
    }
}