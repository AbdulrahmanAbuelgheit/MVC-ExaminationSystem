// Updated ViewModel
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AssignInstructorToCourseVM
{
    public int CourseID { get; set; }
    public int TrackID { get; set; }
    public string CourseName { get; set; }
    public List<DisplayInstructorVM> Instructors { get; set; }
    public int SelectedInstructorId { get; set; }  // For form submission
}
