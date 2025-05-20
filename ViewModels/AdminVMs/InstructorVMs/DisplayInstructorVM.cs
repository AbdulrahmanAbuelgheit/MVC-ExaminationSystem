

namespace ExaminationSystemMVC.ViewModels.AdminVMs.InstructorVMs
{
    public class DisplayInstructorVM
    {
        public int InsID { get; set; }
        public string InsName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Salary { get; set; }
        public int? BranchID { get; set; }
        public string BranchName { get; set; }
        public List<DisplayBranchVM> Branches { get; set; } = new List<DisplayBranchVM>();
        public DisplayBranchVM ManagedBranch { get; set; } = new DisplayBranchVM();
        
        public List<DisplayTrackVM> Tracks { get; set; } = new List<DisplayTrackVM>();
        public DisplayTrackVM SupervisedTrack { get; set; } = new DisplayTrackVM();
        public List<DisplayCourseVM> Courses { get; set; } = new List<DisplayCourseVM>();
    }
}
