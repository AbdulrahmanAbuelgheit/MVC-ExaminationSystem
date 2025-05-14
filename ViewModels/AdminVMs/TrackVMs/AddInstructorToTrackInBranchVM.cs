using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.TrackVMs
{
    public class AddInstructorToTrackInBranchVM
    {
        public int TrackID { get; set; }
        public int BranchID { get; set; }
        [Required(ErrorMessage = "Please select an instructor.")]
        public int InsID { get; set; }
       
    }
}
