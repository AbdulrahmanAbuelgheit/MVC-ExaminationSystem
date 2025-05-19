using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.BranchVMs
{
    public class DisplayBranchVM
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }

        public string Governate { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        [Required]
        public int Tel { get; set; }

        public int ManagerID { get; set; }
        public string ManagerName { get; set; }
        public List<DisplayTrackWithInstructorsVM> Tracks { get; set; } = new();
        public List<DisplayInstructorVM> Instructors { get; set; } = new();

    }
}
