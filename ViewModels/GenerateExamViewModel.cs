using ExaminationSystemMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


public class ExamWithTrackVM
{
    public Exam Exam { get; set; }
    public string CourseName { get; set; }
    public string TrackName { get; set; }
}

public class GenerateExamViewModel
{
    [Required(ErrorMessage = "Please select a course.")]
    public int CrsID { get; set; }

    [Required(ErrorMessage = "Please enter exam duration.")]
    [Range(1, 180, ErrorMessage = "Exam duration must be between 1 and 180 minutes.")]
    public int ExamDuration { get; set; }

    [Required(ErrorMessage = "Please enter number of True/False questions.")]
    [Range(1, 20, ErrorMessage = "Must be between 1 and 20 True/False questions.")]
    public int NumTFQuestions { get; set; }

    [Required(ErrorMessage = "Please enter number of MCQ questions.")]
    [Range(1, 30, ErrorMessage = "Must be between 1 and 30 MCQ questions.")]
    public int NumMCQQuestions { get; set; }

    public IEnumerable<SelectListItem> Courses { get; set; }
}

public class GeneratedExamDetailsViewModel
{
    public int ExamID { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int NumTF { get; set; }
    public int NumMCQ { get; set; }
    public DateTime ExamDateTime { get; set; }
    public int Duration { get; set; }

    public List<ExamQuestionViewModel> Questions { get; set; } = new();
}
public class ExamQuestionViewModel
{
    public int QuestionID { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public int Points { get; set; }
    public int CorrectOption { get; set; }
    public List<string> Options { get; set; } = new();

    public List<int> SelectedQuestionIds { get; set; } = new List<int>();
    public List<Question> AvailableTFQuestions { get; set; } = new List<Question>();
    public List<Question> AvailableMCQQuestions { get; set; } = new List<Question>();
}
