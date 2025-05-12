using System.Data;
using ExaminationSystemMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExaminationSystemMVC.Reposatories
{

    public interface IInstructorRepository
    {
        User GetUserByEmail(string email);
        dynamic GetDashboardData(int instructorId);
        List<Track> GetInstructorTracks(int instructorId);
        List<Course> GetInstructorCourses(int instructorId);
        List<Course> GetCoursesByTrack(int instructorId, int trackId);
        dynamic GetCourseDetails(int courseId);
        List<InstructorCourseSummaryViewModel> GetInstructorCoursesSummary(int instructorId);
        List<Student> GetStudentsInCourse(int courseId);
        List<dynamic> GetCourseStudents(int courseId, int? trackId);
        List<Exam> GetInstructorExams(int instructorId, int? courseId);
        void ToggleExamStatus(int examId);
        GeneratedExamDetailsViewModel GenerateExam(GenerateExamViewModel model);
        GeneratedExamDetailsViewModel GetGeneratedExamDetails(int examId);
        List<Exam> GetCourseExams(int courseId);
        public Instructor GetInstructorByUserId(int userId);
        List<SelectListItem> GetCourseSelectList(int instructorId);

        public string GetCourseName(int courseId);
        public List<Exam> GetInstructorExamsWithDetails(int instructorId, int? courseId);
        public Instructor GetById(int id);
        public int InsertQuestion(string qText, string type, int points, int crsId, int correctOptNum);
        //public int InsertOptions(int qId, string optText1, string optText2, string optText3, string optText4);
        public int InsertOptionsUsingSP(int qId, string optText1, string optText2, string optText3, string optText4);

        public List<DisplayInstructorVM> GetAllWithBranchAndTrack();

        public DisplayInstructorVM GetInstructorEithBranchAndTrackById(int id);

        public EditInstructorViewModel GetInstructorForEdit(int id);
        public Instructor EditInstructor(EditInstructorViewModel model);
        public void DeleteInstructor(int instructorId);

    }


    public class InstructorRepository : IInstructorRepository
    {
        private readonly DBContext _context;

        public InstructorRepository(DBContext context)
        {
            _context = context;
        }

        public List<Instructor> GetAllInstructors()
        {
            return _context.Instructors.Include(i => i.Ins).ToList();
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        public Instructor GetById(int id)
        {
            return _context.Instructors.Include(i => i.Ins) 
                                 .FirstOrDefault(i => i.InsID == id);
        }

        public Instructor GetInstructorByUserId(int userId)
        {
            return _context.Instructors.FirstOrDefault(i => i.InsID == userId);
        }


        public dynamic GetDashboardData(int instructorId)
        {
            var instructor = _context.Instructors
                .Where(i => i.InsID == instructorId)
                .Include(i => i.Crs)
                    .ThenInclude(c => c.Student_Courses)
                .FirstOrDefault();

            if (instructor == null) return null;

            var courseNames = instructor.Crs.Select(c => c.CrsName).ToList();
            var studentCounts = instructor.Crs.Select(c => c.Student_Courses.Count).ToList();

            var totalCourses = instructor.Crs.Count;

            var totalStudents = instructor.Crs
                .SelectMany(c => c.Student_Courses)
                .Select(sc => sc.StdID)
                .Distinct()
                .Count();

            var totalExams = _context.Exams
                .Where(e => e.Crs.Ins.Any(i => i.InsID == instructorId))
                .Count();

            return new
            {
                Name = $"{instructor.Ins.FirstName} {instructor.Ins.LastName}",
                CoursesCount = totalCourses,
                TotalSalary = instructor.Salary,
                CourseNames = courseNames,
                StudentCounts = studentCounts,
                TotalStudents = totalStudents,
                TotalCourses = totalCourses,
                TotalExams = totalExams
            };
        }


        public List<Track> GetInstructorTracks(int instructorId)
        {
            return _context.Tracks
                .Where(t => t.Ins.Any(i => i.InsID == instructorId))
                .Include(t => t.Crs)
                .ToList();
        }

        public List<Course> GetInstructorCourses(int instructorId)
        {

            return _context.Courses
                .Where(c => c.Ins.Any(i => i.InsID == instructorId))
                .ToList();
        }


        public List<Course> GetCoursesByTrack(int instructorId, int trackId)
        {
            return _context.Courses
                .Where(c => c.Tracks.Any(t => t.TrackID == trackId) &&
                            c.Ins.Any(i => i.InsID == instructorId))
                .Include(c => c.Student_Courses)
                .ToList();
        }

        public dynamic GetCourseDetails(int courseId)
        {
            var course = _context.Courses
                .Where(c => c.CrsID == courseId)
                .Include(c => c.Student_Courses)
                .Include(c => c.Exams)
                .FirstOrDefault();

            if (course == null) return null;

            return new
            {
                Name = course.CrsName,
                Description = course.Description,
                Duration = course.Duration,
                NumberOfStudents = course.Student_Courses.Count,
                NumberOfExams = course.Exams.Count
            };
        }

        public List<Student> GetStudentsInCourse(int courseId)
        {
            return _context.Student_Courses
                .Include(sc => sc.Std)
                .Where(sc => sc.CrsID == courseId)
                .Select(sc => sc.Std)
                .ToList();
        }

        public List<dynamic> GetCourseStudents(int courseId, int? trackId)
        {
            var studentsQuery = _context.Student_Courses
                .Include(sc => sc.Std)
                    .ThenInclude(s => s.Std) 
                .Include(sc => sc.Std)
                    .ThenInclude(s => s.Track) 
                .Where(sc => sc.CrsID == courseId);

            if (trackId.HasValue)
            {
                studentsQuery = studentsQuery.Where(sc => sc.Std.TrackID == trackId.Value);
            }

            return studentsQuery
                .Select(sc => new
                {
                    StdID = sc.Std.StdID,
                    Std = new
                    {
                        FirstName = sc.Std.Std.FirstName,
                        LastName = sc.Std.Std.LastName
                    },
                    Track = new
                    {
                        TrackName = sc.Std.Track.TrackName
                    },
                    Grade = sc.Grade
                })
                .ToList<dynamic>();
        }

        public List<Exam> GetInstructorExams(int instructorId, int? courseId)
        {
            var query = _context.Exams
                .Include(e => e.Crs)
                .Where(e => e.Crs.Ins.Any(i => i.InsID == instructorId))
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(e => e.CrsID == courseId.Value);
            }

            return query.ToList(); 
        }

        public void ToggleExamStatus(int examId)
        {
            var exam = _context.Exams.Find(examId);
            if (exam != null)
            {
                exam.IsActive = exam.IsActive == "1" ? "0" : "1";
                _context.SaveChanges();
            }
        }


        public GeneratedExamDetailsViewModel GetGeneratedExamDetails(int examId)
        {
            var exam = _context.Exams
                .Include(e => e.Crs)
                .Include(e => e.QIDs)
                    .ThenInclude(q => q.Questions_Options)
                .FirstOrDefault(e => e.ExamID == examId);

            if (exam == null) return null;

            var questions = exam.QIDs.Select(q => new ExamQuestionViewModel
            {
                QuestionID = q.QID,
                QuestionText = q.QText,
                QuestionType = q.Type,
                Points = q.Points,
                CorrectOption = q.CorrectOptNum,
                Options = q.Questions_Options
                           .OrderBy(o => o.OptNum)
                           .Select(o => o.OptText)
                           .ToList()
            }).ToList();

            return new GeneratedExamDetailsViewModel
            {
                ExamID = exam.ExamID,
                CourseName = exam.Crs.CrsName,
                ExamDateTime = exam.ExamDatetime,
                Duration = exam.ExamDuration,
                NumTF = questions.Count(q => q.QuestionType == "TF"),
                NumMCQ = questions.Count(q => q.QuestionType == "MCQ"),
                Questions = questions
            };
        }

        public List<InstructorCourseSummaryViewModel> GetInstructorCoursesSummary(int instructorId)
        {
            return _context.Courses
                .Where(c => c.Ins.Any(i => i.InsID == instructorId))
                .Select(c => new InstructorCourseSummaryViewModel
                {
                    CourseID = c.CrsID,
                    CourseName = c.CrsName,
                    Description = c.Description,
                    Duration = c.Duration,
                    Tracks = c.Tracks.Select(t => t.TrackName).ToList(),
                    StudentsCount = c.Student_Courses.Count,
                    ExamIDs = c.Exams.Select(e => e.ExamID).ToList()
                })
                .ToList();
        }
        public List<Exam> GetCourseExams(int courseId)
        {
            return _context.Exams
                .Where(e => e.CrsID == courseId)
                .Include(e => e.Crs)
                .ToList();
        }

        public string GetCourseName(int courseId)
        {
            return _context.Courses
                .Where(c => c.CrsID == courseId)
                .Select(c => c.CrsName)
                .FirstOrDefault();
        }

        public List<SelectListItem> GetCourseSelectList(int instructorId)
        {
            return _context.Courses
                .Where(c => c.Ins.Any(i => i.InsID == instructorId))
                .Select(c => new SelectListItem
                {
                    Value = c.CrsID.ToString(),
                    Text = c.CrsName
                })
                .ToList();
        }

        public List<Exam> GetInstructorExamsWithDetails(int instructorId, int? courseId)
        {
            var query = _context.Exams
                .Include(e => e.Crs)
                    .ThenInclude(c => c.Ins) 
                .Where(e => e.Crs.Ins.Any(i => i.InsID == instructorId));

            if (courseId.HasValue)
            {
                query = query.Where(e => e.CrsID == courseId.Value);
            }

            return query.ToList();
        }
        public GeneratedExamDetailsViewModel GenerateExam(GenerateExamViewModel model)
        {
            var examData = new GeneratedExamDetailsViewModel();

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GenerateExam";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@CrsID", model.CrsID));
                    command.Parameters.Add(new SqlParameter("@NumTFQuestions", model.NumTFQuestions));
                    command.Parameters.Add(new SqlParameter("@NumMCQQuestions", model.NumMCQQuestions));
                    command.Parameters.Add(new SqlParameter("@ExamDuration", model.ExamDuration));

                    var reader = command.ExecuteReader();

                    var questions = new List<ExamQuestionViewModel>();
                    int examId = 0;

                    while (reader.Read())
                    {
                        if (examId == 0)
                            examId = Convert.ToInt32(reader["ExamID"]);

                        var q = new ExamQuestionViewModel
                        {
                            QuestionID = Convert.ToInt32(reader["QuestionID"]),
                            QuestionText = reader["QuestionText"].ToString(),
                            QuestionType = reader["QuestionType"].ToString(),
                            Points = Convert.ToInt32(reader["Points"]),
                            CorrectOption = Convert.ToInt32(reader["CorrectOption"]),
                            Options = new List<string>()
                        };

                        for (int i = 1; i <= 4; i++)
                        {
                            string col = $"Option{i}";
                            if (!reader.IsDBNull(reader.GetOrdinal(col)))
                                q.Options.Add(reader[col].ToString());
                        }

                        questions.Add(q);
                    }

                    reader.Close();

                    var courseName = _context.Courses.FirstOrDefault(c => c.CrsID == model.CrsID)?.CrsName ?? "N/A";
                    var exam = _context.Exams.FirstOrDefault(e => e.ExamID == examId);

                    examData = new GeneratedExamDetailsViewModel
                    {
                        ExamID = examId,
                        CourseName = courseName,
                        NumTF = model.NumTFQuestions,
                        NumMCQ = model.NumMCQQuestions,
                        Duration = model.ExamDuration,
                        ExamDateTime = exam?.ExamDatetime ?? DateTime.Now,
                        Questions = questions
                    };
                }
            }

            return examData;
        }

        public int InsertQuestion(string qText, string type, int points, int crsId, int correctOptNum)
        {
            if (type == "T/F" && points != 1)
            {
                throw new InvalidOperationException("True/False questions must have 1 point");
            }
            else if (type == "MCQ" && points != 2)
            {
                throw new InvalidOperationException("MCQ questions must have 2 points");
            }

            var question = new Question
            {
                QText = qText,
                Type = type,
                Points = points,
                CrsID = crsId,
                CorrectOptNum = correctOptNum
            };

            _context.Questions.Add(question);
            _context.SaveChanges();

            return question.QID;
        }

        public int InsertOptionsUsingSP(int qId, string optText1, string optText2, string optText3, string optText4)
        {
            optText1 = string.IsNullOrEmpty(optText1) ? "True" : optText1;
            optText2 = string.IsNullOrEmpty(optText2) ? "False" : optText2;
            optText3 = string.IsNullOrEmpty(optText3) ? null : optText3;
            optText4 = string.IsNullOrEmpty(optText4) ? null : optText4;

            return _context.Database.ExecuteSqlRaw(
                "EXEC QOptions_Insert @QID = {0}, @OptNum1 = {1}, @OptNum2 = {2}, @OptNum3 = {3}, @OptNum4 = {4}, @OptText1 = {5}, @OptText2 = {6}, @OptText3 = {7}, @OptText4 = {8}",
                qId, 1, 2, 3, 4, optText1, optText2, optText3, optText4
            );
        }

        public List<DisplayInstructorVM> GetAllWithBranchAndTrack()
        {
            var instructors = _context.Instructors
                .Include(i => i.Ins)
                    .ThenInclude(u => u.User_PhoneNumbers)
                .Include(i => i.Crs)
                .ToList();

            var instructorVMs = new List<DisplayInstructorVM>();
            foreach (var instructor in instructors)
            {
                var vm = new DisplayInstructorVM
                {
                    InsID = instructor.InsID,
                    InsName = $"{instructor.Ins.FirstName} {instructor.Ins.LastName}",
                    Email = instructor.Ins.Email,
                    PhoneNumber = instructor.Ins.User_PhoneNumbers.FirstOrDefault()?.PhoneNumber.ToString(),
                    Salary = instructor.Salary,
                    Courses = instructor.Crs.Select(c => new DisplayCourseVM
                    {
                        CrsID = c.CrsID,
                        CrsName = c.CrsName
                    }).ToList()
                };

                instructorVMs.Add(vm);
            }
            return instructorVMs;
        }


        public DisplayInstructorVM GetInstructorEithBranchAndTrackById(int id)
        {
            var instructor = _context.Instructors
                .Include(i => i.Ins)
                    .ThenInclude(u => u.User_PhoneNumbers)
                .Include(i => i.BranchesNavigation)
                .Include(i => i.Tracks)
                .Include(i => i.Crs)
                .Include(i => i.Branches)
                .Include(i => i.TracksNavigation)
                .FirstOrDefault(i => i.InsID == id);

            if (instructor == null)
                return null;

            var vm = new DisplayInstructorVM
            {
                InsID = instructor.InsID,
                InsName = $"{instructor.Ins.FirstName} {instructor.Ins.LastName}",
                Email = instructor.Ins.Email,
                PhoneNumber = instructor.Ins.User_PhoneNumbers.FirstOrDefault()?.PhoneNumber.ToString(),
                Salary = instructor.Salary,
                Branches = instructor.BranchesNavigation.Select(b => new DisplayBranchVM
                {
                    BranchID = b.BranchID,
                    BranchName = b.BranchName
                }).ToList(),
                Tracks = instructor.Tracks.Select(t => new DisplayTrackVM
                {
                    TrackID = t.TrackID,
                    TrackName = t.TrackName
                }).ToList(),
                Courses = instructor.Crs.Select(c => new DisplayCourseVM
                {
                    CrsID = c.CrsID,
                    CrsName = c.CrsName
                }).ToList()
            };

            var managedBranch = instructor.Branches.FirstOrDefault();
            if (managedBranch != null)
            {
                vm.ManagedBranch = new DisplayBranchVM
                {
                    BranchID = managedBranch.BranchID,
                    BranchName = managedBranch.BranchName
                };
            }

            var supervisedTrack = instructor.TracksNavigation.FirstOrDefault();
            if (supervisedTrack != null)
            {
                vm.SupervisedTrack = new DisplayTrackVM
                {
                    TrackID = supervisedTrack.TrackID,
                    TrackName = supervisedTrack.TrackName
                };
            }

            return vm;
        }

        public EditInstructorViewModel GetInstructorForEdit(int id)
        {
            var instructor = _context.Instructors
                .Include(i => i.Ins)
                    .ThenInclude(u => u.User_PhoneNumbers)
                .FirstOrDefault(i => i.InsID == id);

            if (instructor == null)
                return null;

            return new EditInstructorViewModel
            {
                InsID = instructor.InsID,
                Salary = instructor.Salary,
                FirstName = instructor.Ins.FirstName,
                LastName = instructor.Ins.LastName,
                Email = instructor.Ins.Email,
                PhoneNumber = instructor.Ins.User_PhoneNumbers.FirstOrDefault()?.PhoneNumber.ToString()
            };
        }

        public Instructor EditInstructor(EditInstructorViewModel model)
        {
            var existingInstructor = _context.Instructors
                .Include(i => i.Ins)
                    .ThenInclude(u => u.User_PhoneNumbers)
                .FirstOrDefault(i => i.InsID == model.InsID);

            if (existingInstructor == null)
                return null;

            existingInstructor.Salary = model.Salary;
            existingInstructor.Ins.FirstName = model.FirstName;
            existingInstructor.Ins.LastName = model.LastName;
            existingInstructor.Ins.Email = model.Email;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                _context.User_PhoneNumbers.RemoveRange(existingInstructor.Ins.User_PhoneNumbers);
                existingInstructor.Ins.User_PhoneNumbers.Add(new User_PhoneNumber
                {
                    UserID = existingInstructor.Ins.UserID,
                    PhoneNumber = int.Parse(model.PhoneNumber)
                });
            }

            _context.SaveChanges();
            return existingInstructor;
        }

        public void DeleteInstructor(int instructorId)
        {
            var instructor = _context.Instructors
                .Include(i => i.Ins)
                .FirstOrDefault(i => i.InsID == instructorId);

            if (instructor != null)
            {
                // remove related User_PhoneNumbers and User
                if (instructor.Ins != null)
                {
                    _context.Instructors.Remove(instructor);

                    var userPhoneNumbers = _context.User_PhoneNumbers
                        .Where(p => p.UserID == instructor.Ins.UserID);
                    _context.User_PhoneNumbers.RemoveRange(userPhoneNumbers);

                    _context.Users.Remove(instructor.Ins);
                }

                _context.Instructors.Remove(instructor);
                _context.SaveChanges();
            }
        }

        public void AddInstructor(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
        }


    }




    //public int InsertOptions(int qId, string optText1, string optText2, string optText3, string optText4)
    //{
    //    var options = new List<Questions_Option>
    //    {
    //        new Questions_Option { QID = qId, OptNum = 1, OptText = optText1 },
    //        new Questions_Option { QID = qId, OptNum = 2, OptText = optText2 },
    //        new Questions_Option { QID = qId, OptNum = 3, OptText = optText3 },
    //        new Questions_Option { QID = qId, OptNum = 4, OptText = optText4 }
    //    };

    //    _context.AddRange(options); 
    //    _context.SaveChanges(); 

    //    return options.Count; 
    //}

}

