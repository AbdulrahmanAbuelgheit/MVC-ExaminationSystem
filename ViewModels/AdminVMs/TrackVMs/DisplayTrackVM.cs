
namespace ExaminationSystemMVC.ViewModels.AdminVMs.TrackVMs
{
    public class DisplayTrackVM
    {
        public int TrackID { get; set; }
        public string TrackName { get; set; }
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public int SupervisorID { get; set; }
        public string SupervisorName { get; set; }

        // Related Branches offering this track
        public List<DisplayBranchVM> Branches { get; set; } = new List<DisplayBranchVM>();

        // Students enrolled in this track
        public List<DisplayStudentVM> Students { get; set; } = new List<DisplayStudentVM>();

        // Instructors teaching courses in this track
        public List<DisplayInstructorVM> Instructors { get; set; } = new List<DisplayInstructorVM>();

        // Courses under this track
        public List<DisplayCourseVM> Courses { get; set; } = new List<DisplayCourseVM>();
    }

}
