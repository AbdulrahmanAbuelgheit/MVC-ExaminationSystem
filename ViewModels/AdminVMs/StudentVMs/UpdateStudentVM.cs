namespace ExaminationSystemMVC.ViewModels.AdminVMs.StudentVMs
{
    public class UpdateStudentVM : CreateStudentVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string IsActive { get; set; }

    }
}
