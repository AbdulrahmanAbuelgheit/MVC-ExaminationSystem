using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.TrackVMs
{
    public class AddInstructorToTrackVM
    {
        public int TrackID { get; set; }
        [Required(ErrorMessage = "Please select an instructor.")]
        public int InsID { get; set; }
       
    }
}
