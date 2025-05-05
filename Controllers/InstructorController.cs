using ExaminationSystemMVC.Models.JWT;
using ExaminationSystemMVC.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Instructor")]

    public class InstructorController : Controller
    {
        private readonly IInstructorRepository _repository;
        private  int InsID;
        private readonly JwtHelper _jwt;
        private readonly UsersRepo _userRepository;

        public InstructorController(IInstructorRepository repository, JwtHelper jwt ,UsersRepo userrepository)
        {
            _repository = repository;
            InsID = GetCurrentUserIdFromDatabase();
            _jwt = jwt;
            _userRepository=userrepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        private string GetCurrentUserEmail()
        {
            if (Request == null)
            {
                return null;
            }

            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return null;

            var principal = _jwt.ValidateToken(token);
            if (principal == null)
                return null;

            var emailClaim = principal.FindFirst(ClaimTypes.Email);
            return emailClaim?.Value;
        }


        private int GetCurrentUserIdFromDatabase()
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email))
                return 0;

            var user = _repository.GetUserByEmail(email); 
            return user?.UserID ?? 0;
        }

        public IActionResult Tracks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);
            var tracks = _repository.GetInstructorTracks(instructor.InsID);
            return View(tracks);
        }

        public IActionResult Courses(int trackId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);

            var courses = _repository.GetCoursesByTrack(instructor.InsID, trackId);
            return View(courses);
        }

        public IActionResult Students(int courseId)
        {

            var students = _repository.GetStudentsInCourse(courseId);
            return View(students);
        }

        public IActionResult Exams(int? courseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);
            var exams = _repository.GetInstructorExams(instructor.InsID, courseId).ToList();

            ViewBag.Courses = _repository.GetCourseSelectList(instructor.InsID) ?? new List<SelectListItem>();
            ViewBag.CourseId = courseId;

            if (courseId.HasValue)
            {
                ViewBag.CourseName = _repository.GetCourseName(courseId.Value);
            }

            return View(exams); 
        }


        public IActionResult DeleteExam(int id)
        {
            _repository.ToggleExamStatus(id);
            return RedirectToAction("Exams");
        }

        public IActionResult GenerateExam()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);
            var model = new GenerateExamViewModel
            {
                Courses = _repository.GetCourseSelectList(instructor.InsID)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult GenerateExam(GenerateExamViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);
            if (!ModelState.IsValid)
            {
                model.Courses = _repository.GetCourseSelectList(instructor.InsID);
                return View(model);
            }

            var result = _repository.GenerateExam(model);
            return View("GeneratedExamDetails", result);
        }

        public IActionResult GeneratedExamDetails(int id)
        {
            var examDetails = _repository.GetGeneratedExamDetails(id);
            return View(examDetails);
        }

        public IActionResult InstructorCoursesSummary()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);

            var courses = _repository.GetInstructorCoursesSummary(instructor.InsID);
            return View(courses);
        }

        public IActionResult CourseStudents(int courseId, int? trackId)
        {
            var students = _repository.GetCourseStudents(courseId, trackId);
            ViewBag.CourseName = _repository.GetInstructorCourses(InsID)
                .FirstOrDefault(c => c.CrsID == courseId)?.CrsName;
            ViewBag.CourseId = courseId;

            var tracks = _repository.GetStudentsInCourse(courseId)
                .Select(s => s.Track)
                .Distinct()
                .ToList();

            ViewBag.TrackList = new SelectList(tracks, "TrackID", "TrackName");

            return View(students);
        }

        public IActionResult CourseExams(int courseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId);

            var exams = _repository.GetCourseExams(courseId);

            ViewBag.CourseName = _repository.GetCourseName(courseId);

            ViewBag.Courses = _repository.GetCourseSelectList(instructor.InsID);

            ViewBag.CourseId = courseId;

            return View(exams);
        }

        public IActionResult CourseDetails(int courseId)
        {
            var courseDetails = _repository.GetCourseDetails(courseId);
            if (courseDetails == null)
            {
                return NotFound();
            }
            ViewBag.CourseId = courseId;
            return View(courseDetails); 
        }

        [Authorize(Roles = "Instructor")]
        public IActionResult Dashboard()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var instructor = _repository.GetById(userId); 

            if (instructor == null)
            {
                return NotFound();
            }

            var dashboardData = _repository.GetDashboardData(instructor.InsID); 

            if (dashboardData == null)
            {
                return View("Error"); 
            }

            return View(dashboardData);
        }



    }
}