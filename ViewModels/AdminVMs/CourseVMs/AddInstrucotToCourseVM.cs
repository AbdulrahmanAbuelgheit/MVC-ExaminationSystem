using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.CourseVMs
{
    public class AddInstrucotToCourseVM
    {

        public int CrsId { get; set; }
        [Required(ErrorMessage = "Please select an instructor.")]
        public int InsID { get; set; }

    }
}
