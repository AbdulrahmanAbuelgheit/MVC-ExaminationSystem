using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExaminationSystemMVC.ViewModels
{
    public class QuestionViewModel
    {
        [Required(ErrorMessage = "Question text is required")]
        public string QText { get; set; }

        [Required(ErrorMessage = "Question type is required")]
        public string Type { get; set; } // "T/F" or "MCQ"

        [Required(ErrorMessage = "Points are required")]
        public int Points { get; set; }

        [Required(ErrorMessage = "Course is required")]
        public int CrsID { get; set; }

        [Required(ErrorMessage = "Correct option is required")]
        [Range(1, 4, ErrorMessage = "Correct option must be between 1 and 4")]
        public int CorrectOptNum { get; set; }

        public string OptText1 { get; set; }
        public string OptText2 { get; set; }
        public string OptText3 { get; set; }
        public string OptText4 { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == "T/F" && Points != 1)
            {
                yield return new ValidationResult(
                    "True/False questions must be worth exactly 1 point",
                    new[] { nameof(Points) });
            }
            else if (Type == "MCQ" && Points != 2)
            {
                yield return new ValidationResult(
                    "MCQ questions must be worth exactly 2 points",
                    new[] { nameof(Points) });
            }

            if (Type == "T/F")
            {
                if (string.IsNullOrWhiteSpace(OptText1) || OptText1 != "True")
                {
                    yield return new ValidationResult(
                        "Option 1 must be 'True' for True/False questions",
                        new[] { nameof(OptText1) });
                }

                if (string.IsNullOrWhiteSpace(OptText2) || OptText2 != "False")
                {
                    yield return new ValidationResult(
                        "Option 2 must be 'False' for True/False questions",
                        new[] { nameof(OptText2) });
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(OptText1))
                {
                    yield return new ValidationResult(
                        "Option 1 is required for MCQ questions",
                        new[] { nameof(OptText1) });
                }

                if (string.IsNullOrWhiteSpace(OptText2))
                {
                    yield return new ValidationResult(
                        "Option 2 is required for MCQ questions",
                        new[] { nameof(OptText2) });
                }
            }
        }
    }
}