using System.Data;
using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace ExaminationSystemMVC.Reposatories
{
    public  class StudentRepo : GenericRepo<Student>
    {
        public StudentRepo(DBContext Db) : base(Db)
        {
        }

        public List<Student> GetAllWithBranchAndTrack()
        {
            return Db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).ToList();
        }

        public Student GetByIdWithBranchAndTrack(int id)
        {
            return Db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).FirstOrDefault(s => s.StdID == id);
        }
        public IEnumerable<Student> GetByBranchId(int branchId)
        {
            return Db.Students
                .Include(s => s.Std)
                .Where(s => s.BranchID == branchId)
                .ToList();
        }
        // In StudentRepository.cs
        public IEnumerable<Student> GetByTrackAndBranch(int trackId, int branchId)
        {
            return Db.Students
                .Include(s => s.Std)
                .Where(s => s.TrackID == trackId && s.BranchID == branchId)
                .ToList();
        }

        public IEnumerable<Student> GetByBranch(int branchId)
        {
            return Db.Students
                .Include(s => s.Std)
                .Where(s => s.BranchID == branchId)
                .ToList();
        }
        public void SoftDelete(int id)
        {
            var student = GetById(id);
            if (student != null)
            {
                student.IsActive = "F";
                Update(student);
            }
        }

        public Student getById(int id)
        {
            return Db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).FirstOrDefault(s => s.StdID == id);
        }

        public Student getByEmail(string email)
        {
            return Db.Students.Include(s => s.Std).FirstOrDefault(s => s.Std.Email == email);
        }
        public List<Student_Course> getStudentCourse(int id)
        {
            return Db.Student_Courses
                .Where(sc => sc.StdID == id)
                .Include(sc => sc.Crs)
                .ToList();
        }

        public List<Exam> getStudentUpcomingExams(int id)
        {
            var studentCourses = Db.Student_Courses
                .Where(sc => sc.StdID == id)
                .Select(sc => sc.CrsID)
                .ToList();

            if (!studentCourses.Any())
                return new List<Exam>();

            var availableExams = Db.Exams
                .Include(e => e.Crs)
                .Where(e => studentCourses.Contains(e.CrsID) && e.Expire_Date > DateTime.Now)
                .ToList();

            var completedExamIds = Db.Student_Exams
                .Where(se => se.StdID == id)
                .Select(se => se.ExamID)
                .ToList();

            return availableExams
                .Where(e => !completedExamIds.Contains(e.ExamID))
                .ToList();
        }

        public List<Student_Exam> getStudentCompletedExams(int id)
        {
            return Db.Student_Exams
                .Where(se => se.StdID == id)
                .Include(se => se.Exam)
                    .ThenInclude(e => e.Crs)
                .Include(se => se.Exam.QIDs)
                .ToList();
        }
        public List<ExamQuestionWithAnswersVM> GetExamQuestionsWithAnswers(int examId, int studentId)
        {
            var results = new List<ExamQuestionWithAnswersVM>();

            var connectionString = Db.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetExamQuestionsWithOptionsWithStdAnswer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@ExamID", examId));
                    command.Parameters.Add(new SqlParameter("@StdID", studentId));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ExamQuestionWithAnswersVM
                            {
                                QID = reader.GetInt32(reader.GetOrdinal("QID")),
                                QText = reader.GetString(reader.GetOrdinal("QText")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                Options = reader.GetString(reader.GetOrdinal("Options")),
                                Points = reader.GetInt32(reader.GetOrdinal("Points")),
                                CrsID = reader.GetInt32(reader.GetOrdinal("CrsID")),
                            };

                            // Handle CorrectOptNum
                            var correctOptNumOrdinal = reader.GetOrdinal("CorrectOptNum");
                            if (!reader.IsDBNull(correctOptNumOrdinal))
                            {
                                item.CorrectOptNum = reader.GetValue(correctOptNumOrdinal).ToString();
                            }

                            // Handle StudentAnswer
                            var studentAnswerOrdinal = reader.GetOrdinal("StudentAnswer");
                            if (!reader.IsDBNull(studentAnswerOrdinal))
                            {
                                item.StudentAnswer = reader.GetValue(studentAnswerOrdinal).ToString();
                            }

                            results.Add(item);
                        }
                    }
                }
            }

            return results;
        }

        public Exam GetExamById(int examId)
        {
            return Db.Exams
                .Include(e => e.Crs)
                .FirstOrDefault(e => e.ExamID == examId);
        }

        // Get exam questions for a specific exam
        public List<ExamQuestion> GetExamQuestions(int examId)
        {
            var exam = Db.Exams
                .Include(e => e.QIDs)
                .FirstOrDefault(e => e.ExamID == examId);

            if (exam == null)
                return new List<ExamQuestion>();

            var result = new List<ExamQuestion>();

            foreach (var question in exam.QIDs)
            {
                var options = Db.Questions_Options
                    .Where(o => o.QID == question.QID)
                    .OrderBy(o => o.OptNum)
                    .ToList();

                result.Add(new ExamQuestion
                {
                    Question = question,
                    Options = options
                });
            }

            return result;
        }

        // Submit exam answers
        public int SubmitExam(int studentId, int examId, Dictionary<int, string> answers)
        {
            var existingSubmission = Db.Student_Exams
                .FirstOrDefault(se => se.StdID == studentId && se.ExamID == examId);

            if (existingSubmission != null)
                return -1;

            var exam = Db.Exams
                .Include(e => e.QIDs)
                .FirstOrDefault(e => e.ExamID == examId);

            if (exam == null)
                return -2; 

            int totalScore = 0;

            foreach (var question in exam.QIDs)
            {
                if (answers.TryGetValue(question.QID, out string selectedAnswer))
                {
                    Db.Exam_Student_Questions.Add(new Exam_Student_Question
                    {
                        ExamID = examId,
                        StdID = studentId,
                        QID = question.QID,
                        SelectedOpt = selectedAnswer
                    });

                    int correctOption = question.CorrectOptNum;
                    if (selectedAnswer == correctOption.ToString())
                    {
                        totalScore += question.Points;
                    }
                }
            }

            var studentExam = new Student_Exam
            {
                StdID = studentId,
                ExamID = examId,
                Score = totalScore
            };

            Db.Student_Exams.Add(studentExam);
            Db.SaveChanges();

            return totalScore;
        }

    }
}
