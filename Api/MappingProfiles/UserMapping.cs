using AutoMapper;
using Commands.Users;
using Domain.Entities;
using ViewModel.Users;

namespace Api.MappingProfiles
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UsersViewModel>();

            CreateMap<User, UserViewModel>();

            CreateMap<SaveUserCommand, User>()
                .ForMember(dest => dest.Password, opt => opt.Condition(src => src.Id == 0));
        }
    }
}
