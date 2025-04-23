namespace ExaminationSystemMVC.DTOs.AdminDTOs.TrackDTOs
{
    public class CreateTrackDTO
    {
        public string TrackName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public int SupervisorName { get; set; }
        public List<int> BranchIDs { get; set; } = new List<int>();
        public List<int> CourseIDs { get; set; } = new List<int>();
    }
}
