    using AutoMapper;
    using ExaminationSystemMVC.DTOs.UserDTO;
    using ExaminationSystemMVC.Models;
using static ExaminationSystemMVC.DTOs.AuthDTO.Auth;

    namespace ExaminationSystemMVC.MappingConfig
    {
        public class MappingProfile:Profile
        {
            public MappingProfile()
            {
                CreateMap<User,DisplayUserVM>();
                CreateMap<RegisterUserVM, User>();


        }
    }
    }
