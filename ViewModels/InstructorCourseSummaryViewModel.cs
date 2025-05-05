public class InstructorCourseSummaryViewModel
{
    public int CourseID { get; set; }  
    public string CourseName { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }

    public List<string> Tracks { get; set; } = new();
    public int StudentsCount { get; set; }
    public List<int> ExamIDs { get; set; } = new();
}
