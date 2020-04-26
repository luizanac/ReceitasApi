using AutoMapper;
using ReceitasApi.Dtos.Account;
using ReceitasApi.Dtos.User;
using ReceitasApi.Entities;

namespace ReceitasApi.AutoMapper {
    public class DomainToDtoMappingProfile : Profile {
        public DomainToDtoMappingProfile () {

            //User
            CreateMap<User, CurrentUserDto> ();
            CreateMap<User, UserBasicDto> ();
            CreateMap<User, UserDto> ();
            
        }
    }
}