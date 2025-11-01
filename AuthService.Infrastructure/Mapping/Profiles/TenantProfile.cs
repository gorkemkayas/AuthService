using AuthService.Domain.Entities;
using AutoMapper;

namespace AuthService.Infrastructure.Mapping.Profiles
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<Tenant, AuthService.Infrastructure.Persistance.Entities.Tenant>()
                .ReverseMap();
        }
    }
}
