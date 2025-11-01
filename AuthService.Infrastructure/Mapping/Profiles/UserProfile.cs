using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistance.Entities;
using AutoMapper;

namespace AuthService.Infrastructure.Mapping.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ApplicationUser>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
                .ReverseMap();
        }
    }
}
