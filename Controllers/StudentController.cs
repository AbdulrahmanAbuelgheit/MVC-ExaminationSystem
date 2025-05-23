﻿using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.Models.JWT;
using ExaminationSystemMVC.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public StudentRepo StudentRepo { get; set; }
        private readonly JwtHelper _jwtHelper;

        public StudentController(StudentRepo _StudentRepo, JwtHelper jwtHelper)
        {
            StudentRepo = _StudentRepo;
            _jwtHelper = jwtHelper;
        }

        private int GetCurrentStudentId()
        {
            var token = Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                var principal = _jwtHelper.ValidateToken(token);
                if (principal != null)
                {
                    // get student id claim first
                    var studentIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "StudentId");
                    if (studentIdClaim != null && int.TryParse(studentIdClaim.Value, out int studentId))
                    {
                        return studentId;
                    }


                    var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        var student = StudentRepo.getByEmail(emailClaim.Value);
                        if (student != null)
                        {
                            return student.StdID;
                        }
                    }
                }
            }

            return 0;
        }

        [Route("Student/Home")]
        public IActionResult StudentHome()
        {
            ViewData["Layout"] = "~/Views/Shared/StudentLayout.cshtml";
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Dashboard()
        {
            int studentId = GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var student = StudentRepo.getById(studentId);
            if (student == null)
                return NotFound();

            return View(student);
        }

        public IActionResult Index()
        {
            int studentId = GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            return RedirectToAction("Dashboard");
        }

        // student details
        

        // student course
        public IActionResult Courses(int? id = null)
        {
            int studentId = id ?? GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var studentCourses = StudentRepo.getStudentCourse(studentId);
            if (studentCourses == null)
                return NotFound();

            var student = StudentRepo.getById(studentId);
            ViewBag.StudentId = studentId;
            ViewBag.StudentName = student?.Std?.FirstName + " " + student?.Std?.LastName;

            return View(studentCourses);
        }

        // student exam 
        public IActionResult Exams(int? id = null)
        {
            int studentId = id ?? GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var upcomingExams = StudentRepo.getStudentUpcomingExams(studentId);
            var completedExams = StudentRepo.getStudentCompletedExams(studentId);

            if (upcomingExams == null || completedExams == null)
                return NotFound();

            var student = StudentRepo.getById(studentId);
            ViewBag.StudentId = studentId;
            ViewBag.studentName = student?.Std?.FirstName;

            ViewBag.UpcomingExams = upcomingExams
            .OrderBy(e => e.ExamDatetime > DateTime.Now)
            .ThenBy(e => e.ExamDatetime)
            .ToList();
            ViewBag.CompletedExams = completedExams.OrderByDescending(se => se.Exam.ExamDatetime).ToList();

            ViewBag.UpcomingExamsCount = upcomingExams.Count;
            ViewBag.CompletedExamsCount = completedExams.Count;

            return View();

        }
        public IActionResult ExamDetails(int examId, int? id = null)
        {
            int studentId = id ?? GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var examDetails = StudentRepo.GetExamQuestionsWithAnswers(examId, studentId);

            if (examDetails == null || !examDetails.Any())
                return NotFound();

            var exam = StudentRepo.GetExamById(examId);
            var student = StudentRepo.getById(studentId);

            ViewBag.ExamTitle = $"{exam.Crs.CrsName} Exam";
            ViewBag.StudentName = $"{student.Std.FirstName} {student.Std.LastName}";
            ViewBag.ExamDate = exam.ExamDatetime.ToString("MMMM d, yyyy");

            return View(examDetails);
        }

        public IActionResult TakeExam(int id)
        {
            int studentId = GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var exam = StudentRepo.Db.Exams
                .Include(e => e.Crs)
                .FirstOrDefault(e => e.ExamID == id);

            if (exam == null)
                return NotFound();

            if (exam.ExamDatetime > DateTime.Now)
            {
                return RedirectToAction("Exams");
            }

            if (exam.Expire_Date < DateTime.Now)
            {
                return RedirectToAction("Exams");
            }

            var existingExam = StudentRepo.Db.Student_Exams
                .FirstOrDefault(se => se.StdID == studentId && se.ExamID == id);

            if (existingExam != null)
            {
                return RedirectToAction("Exams");
            }

            var examQuestions = StudentRepo.GetExamQuestions(id);

            if (!examQuestions.Any())
            {
                return RedirectToAction("Exams");
            }

            ViewBag.ExamID = id;
            ViewBag.ExamName = exam.Crs.CrsName;
            ViewBag.Duration = exam.ExamDuration;
            ViewBag.EndTime = DateTime.Now.AddMinutes(exam.ExamDuration);

            return View(examQuestions);
        }

        [HttpPost]
        public IActionResult SubmitExam(int examId, IFormCollection form)
        {
            int studentId = GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var answers = new Dictionary<int, string>();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("question_"))
                {
                    string questionIdStr = key.Substring(9); 
                    if (int.TryParse(questionIdStr, out int questionId))
                    {
                        answers[questionId] = form[key];
                    }
                }
            }

            // Submit the exam and get the score
            int score = StudentRepo.SubmitExam(studentId, examId, answers);

            if (score < 0)
            {
                return RedirectToAction("Exams");
            }


            return RedirectToAction("ExamResult", new { id = examId });
        }

        public IActionResult ExamResult(int id)
        {
            int studentId = GetCurrentStudentId();
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var studentExam = StudentRepo.Db.Student_Exams
                .Include(se => se.Exam)
                .ThenInclude(e => e.Crs)
                .FirstOrDefault(se => se.StdID == studentId && se.ExamID == id);

            if (studentExam == null)
            {
                return RedirectToAction("Exams");
            }

            return View(studentExam);
        }


    }
}