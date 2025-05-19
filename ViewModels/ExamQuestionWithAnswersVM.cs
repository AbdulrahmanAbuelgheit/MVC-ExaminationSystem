namespace ExaminationSystemMVC.ViewModels
{
    public class ExamQuestionWithAnswersVM
    {
        public int QID { get; set; }
        public string QText { get; set; }
        public string Type { get; set; }
        public string Options { get; set; }
        public int Points { get; set; }
        public object CorrectOptNum { get; set; }
        public object StudentAnswer { get; set; }
        public string CorrectAnswer => CorrectOptNum?.ToString();
        public string StudentAnswerText => StudentAnswer?.ToString();
        public int CrsID { get; set; }
    }
}
