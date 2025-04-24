using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystemMVC.Controllers
{
    //[Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IStudentRepo StudentRepo { get; set; }
        public StudentController(IStudentRepo _StudentRepo) 
        {
            StudentRepo = _StudentRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        // student details
        public IActionResult Details(int id) 
        {
            if(id == 0)
                return NotFound();

            var student = StudentRepo.getById(id);
            if(student == null)
                return NotFound();

            return View(student);
        }

        // student course
        public IActionResult Courses(int id)
        {
            if (id == 0)
                return NotFound();

            var studentCourses = StudentRepo.getStudentCourse(id);
            if (studentCourses == null || !studentCourses.Any())
                return NotFound();

            var student = StudentRepo.getById(id);
            ViewBag.StudentId = id;
            ViewBag.StudentName = student?.Std?.FirstName + " " + student?.Std?.LastName;

            return View(studentCourses);
        }

        // student exam 
        public IActionResult Exams(int id) 
        {
            if (id == 0)
                return NotFound();

            var studentExam = StudentRepo.getStudentExam(id);
            if(studentExam == null)
                return NotFound();

            ViewBag.StudentId = id;
            ViewBag.studentName = StudentRepo.getById(id)?.Std?.FirstName;

            ViewBag.UpcomingExamsCount = studentExam.Count(e => e.Exam.ExamDatetime > DateTime.Now);
            ViewBag.CompletedExamsCount = studentExam.Count(e => e.Exam.ExamDatetime <= DateTime.Now);
            return View(studentExam);
        }
    }
}
