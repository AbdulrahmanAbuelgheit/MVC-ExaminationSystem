using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.StudentVMs
{
    public class CreateStudentVM
    {
        [Required]
        public int StdID { get; set; }
        [Required]
        public int TrackID { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public int IntakeYear { get; set; }

    }
}
