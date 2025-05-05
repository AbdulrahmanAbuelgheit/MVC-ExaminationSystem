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
    [Required]
    [Display(Name = "Course")]
    public int CrsID { get; set; }

    [Required]
    [Display(Name = "T/F Questions")]
    public int NumTFQuestions { get; set; }

    [Required]
    [Display(Name = "MCQ Questions")]
    public int NumMCQQuestions { get; set; }

    [Display(Name = "Duration (minutes)")]
    public int ExamDuration { get; set; } = 60;

    public List<SelectListItem> Courses { get; set; } = new();
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
}
