using AuthService.Application.Specifications;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistance.Entities;
using AutoMapper;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Mapping
{
    public static class SpecificationMapper
    {
        //public static Specification<ApplicationUser> MapToInfrastructureSpec(
        //    Specification<User> domainSpec,
        //    IMapper mapper)
        //{
        //    if (domainSpec == null) return null;

        //    // Manuel dönüştürme: Expression tree'yi yeniden yazmak
        //    Expression<Func<ApplicationUser, bool>> newCriteria = appUser =>
        //        domainSpec.Criteria.Compile().Invoke(mapper.Map<User>(appUser));

        //    return new Specification<ApplicationUser>
        //    {
        //        Criteria = newCriteria,
        //        Includes = domainSpec.Includes,
        //    };
        //}
    }

}
