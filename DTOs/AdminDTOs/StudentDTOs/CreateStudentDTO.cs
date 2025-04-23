using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.StudentDTOs
{
    public class CreateStudentDTO
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
