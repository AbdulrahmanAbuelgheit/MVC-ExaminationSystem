using AutoMapper;
using ExaminationSystemMVC.DTOs.UserDTO;
using ExaminationSystemMVC.Models;

namespace ExaminationSystemMVC.MappingConfig
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User,DisplayUserDTO>();

        }
    }
}
