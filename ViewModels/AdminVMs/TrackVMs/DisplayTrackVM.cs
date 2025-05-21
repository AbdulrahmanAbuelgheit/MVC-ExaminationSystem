
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
        public List<DisplayBranchVM> Branches { get; set; } = new List<DisplayBranchVM>();
        public List<DisplayStudentVM> Students { get; set; } = new List<DisplayStudentVM>();
        public List<DisplayInstructorVM> Instructors { get; set; } = new List<DisplayInstructorVM>();
        public List<DisplayCourseVM> Courses { get; set; } = new List<DisplayCourseVM>();
    }

}
