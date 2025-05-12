using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.StudentVMs
{
    public class EditStudentVM : CreateStudentVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required]
        [StringLength(1)]
        public string IsActive { get; set; } = "T";
        public SelectList AvailableTracks { get; set; }


    }
}
