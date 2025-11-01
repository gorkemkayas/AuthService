using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Specifications;
using AuthService.Application.Specifications;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistance.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class UserRepository(GenericRepository<ApplicationUser> genericRepository, UserManager<ApplicationUser> userManager, IMapper mapper) : IUserRepository
    {
        private readonly GenericRepository<ApplicationUser> _genericRepository = genericRepository;
        //public async Task<IEnumerable<ApplicationUser>?> GetAllUsersAsync(ISpecification<User>? specification)
        //{
        //    var mappedSpecification = new Specification<ApplicationUser>();
        //    mappedSpecification.Criteria = specification?.Criteria != null
        //        ? user => specification.Criteria(mapper.Map<User>(user))
        //        : null;

        //    return await _genericRepository.GetAllAsync(MappedSpecification);
        //}


    }
}
