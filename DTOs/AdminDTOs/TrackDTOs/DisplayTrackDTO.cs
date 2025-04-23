using ExaminationSystemMVC.DTOs.AdminDTOs.BranchDTOs;
using ExaminationSystemMVC.DTOs.AdminDTOs.CourseDTOs;
using ExaminationSystemMVC.DTOs.AdminDTOs.InstructorDTOs;
using ExaminationSystemMVC.DTOs.AdminDTOs.StudentDTOs;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.TrackDTOs
{
    public class DisplayTrackDTO
    {
        public int TrackID { get; set; }
        public string TrackName { get; set; }
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public int SupervisorID { get; set; }
        public string SupervisorName { get; set; }

        // Related Branches offering this track
        public List<DisplayBranchDTO> Branches { get; set; } = new List<DisplayBranchDTO>();

        // Students enrolled in this track
        public List<DisplayStudentDTO> Students { get; set; } = new List<DisplayStudentDTO>();

        // Instructors teaching courses in this track
        public List<DisplayInstructorDTO> Instructors { get; set; } = new List<DisplayInstructorDTO>();

        // Courses under this track
        public List<DisplayCourseDTO> Courses { get; set; } = new List<DisplayCourseDTO>();
    }

}
