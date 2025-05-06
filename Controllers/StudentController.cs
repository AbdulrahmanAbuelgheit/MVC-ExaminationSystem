using ExaminationSystemMVC.Models;
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
        public IStudentRepo StudentRepo { get; set; }
        private readonly JwtHelper _jwtHelper;

        public StudentController(IStudentRepo _StudentRepo, JwtHelper jwtHelper)
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
            if (studentCourses == null || !studentCourses.Any())
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

            var studentExam = StudentRepo.getStudentExam(studentId);
            if (studentExam == null)
                return NotFound();

            var student = StudentRepo.getById(studentId);
            ViewBag.StudentId = studentId;
            ViewBag.studentName = student?.Std?.FirstName;

            ViewBag.UpcomingExamsCount = studentExam.Count(e => e.Exam.ExamDatetime > DateTime.Now);
            ViewBag.CompletedExamsCount = studentExam.Count(e => e.Exam.ExamDatetime <= DateTime.Now);
            return View(studentExam);
        }
    }
}