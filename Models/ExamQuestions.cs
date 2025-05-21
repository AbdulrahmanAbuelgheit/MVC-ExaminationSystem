namespace ExaminationSystemMVC.Models
{
    public class ExamQuestion
    {
        public Question Question { get; set; }
        public List<Questions_Option> Options { get; set; }
    }
}
