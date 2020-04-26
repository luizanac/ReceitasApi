using AutoMapper;
using ReceitasApi.Dtos.User;
using ReceitasApi.Entities;

namespace ReceitasApi.AutoMapper {
    public class DtoToDomainMappingProfile : Profile {
        public DtoToDomainMappingProfile () {

            //User
            CreateMap<CreateUserDto, User> ();
            CreateMap<UpdateUserDto, User> ();
        }
    }
}