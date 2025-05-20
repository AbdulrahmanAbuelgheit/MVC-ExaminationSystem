namespace ExaminationSystemMVC.ViewModels.Course
{
    public class CourseDetailsVM
    {
        public int CourseID { get; set; }
        public int TrackID { get; set; }
        public int Duration { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public List<DisplayInstructorVM> Instructors { get; set; } = new List<DisplayInstructorVM>();
    }
}
