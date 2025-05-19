namespace ExaminationSystemMVC.ViewModels.AdminVMs.TrackVMs
{
    public class DisplayTrackWithInstructorsVM
    {
        public int TrackID { get; set; }
        public string TrackName { get; set; }
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public Instructor Supervisor { get; set; }
        public List<Instructor> Instructors { get; set; }
    }
}
