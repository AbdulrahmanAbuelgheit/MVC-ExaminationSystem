namespace ExaminationSystemMVC.DTOs.AdminDTOs.StudentDTOs
{
    public class DisplayStudentDTO
    {
        public int StdID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        //public string Phone { get; set; }
        public string TrackName { get; set; }
        public string BranchName { get; set; }
        public int IntakeYear { get; set; }
        public string IsActive { get; set; }
    }
}
