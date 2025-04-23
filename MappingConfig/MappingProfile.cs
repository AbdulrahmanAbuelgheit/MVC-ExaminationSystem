using AutoMapper;

namespace ExaminationSystemMVC.MappingConfig
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User,DisplayUserDTO>();
            CreateMap<Student, DisplayStudentDTO>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Std.FirstName + ' ' + src.Std.LastName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Std.Email))
               .ForMember(dest => dest.TrackName, opt => opt.MapFrom(src => src.Track.TrackName))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == "T" ? "Active" : "Inactive"));

            CreateMap<CreateStudentDTO, Student>();
            CreateMap<Student, UpdateStudentDTO>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Std.FirstName + ' ' + src.Std.LastName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Std.Email))
               .ReverseMap();

            CreateMap<Branch, DisplayBranchDTO>()
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.Ins.FirstName + ' ' + src.Manager.Ins.LastName))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));

            CreateMap<Branch, CreateBranchDTO>().ReverseMap();
            CreateMap<Branch, UpdateBranchDTO>().ReverseMap();

            CreateMap<Track, DisplayTrackDTO>()
                .ForMember(dest => dest.SupervisorID, opt => opt.MapFrom(src => src.Supervisor.InsID))
                .ForMember(dest => dest.SupervisorName, opt => opt.MapFrom(src => src.Supervisor.Ins.FirstName + ' ' + src.Supervisor.Ins.LastName))
                .ForMember(dest => dest.Instructors, opt => opt.MapFrom(src => src.Ins))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Crs))
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students))
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.Branches))
                .ReverseMap();


            CreateMap<Instructor, DisplayInstructorDTO>()
                .ForMember(dest => dest.InsName, opt => opt.MapFrom(src => src.Ins.FirstName + ' ' + src.Ins.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Ins.Email));
                
            CreateMap<Course, DisplayCourseDTO>();


        }
    }
}
