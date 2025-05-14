using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.TrackVMs
{

    public class AddInstructorToTrackVM
    {
        public int TrackID { get; set; }

        [Required(ErrorMessage = "Please select an instructor.")]
        [Display(Name = "Instructor")]
        public int InsID { get; set; }
        public int BranchID { get; set; }

        public SelectList AvailableInstructors { get; set; }
    }
}
