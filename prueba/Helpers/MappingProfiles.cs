using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using prueba.Dto;
using prueba.Entities;

namespace prueba.Helpers
{
    public class MappingProfiles: Profile
    {
         public MappingProfiles()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
        
    }
}