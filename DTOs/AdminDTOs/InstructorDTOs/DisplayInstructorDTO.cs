using ExaminationSystemMVC.DTOs.AdminDTOs.BranchDTOs;
using ExaminationSystemMVC.DTOs.AdminDTOs.CourseDTOs;
using ExaminationSystemMVC.DTOs.AdminDTOs.TrackDTOs;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.InstructorDTOs
{
    public class DisplayInstructorDTO
    {
        public int InsID { get; set; }
        public string InsName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Salary { get; set; }
        public List<DisplayBranchDTO> Branches { get; set; } = new List<DisplayBranchDTO>();
        public DisplayBranchDTO ManagedBranch { get; set; } = new DisplayBranchDTO();
        
        public List<DisplayTrackDTO> Tracks { get; set; } = new List<DisplayTrackDTO>();
        public DisplayTrackDTO SupervisedTrack { get; set; } = new DisplayTrackDTO();
        public List<DisplayCourseDTO> Courses { get; set; } = new List<DisplayCourseDTO>();
    }
}
