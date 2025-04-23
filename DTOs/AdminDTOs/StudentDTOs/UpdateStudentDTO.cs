namespace ExaminationSystemMVC.DTOs.AdminDTOs.StudentDTOs
{
    public class UpdateStudentDTO : CreateStudentDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string IsActive { get; set; }

    }
}
