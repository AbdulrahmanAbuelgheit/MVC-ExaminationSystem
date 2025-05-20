using ExaminationSystemMVC.Models.JWT;
using ExaminationSystemMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly IInstructorRepository _repository;
        private readonly JwtHelper _jwt;
        private readonly UsersRepo _userRepository;
        private bool _isManager;

        public InstructorController(IInstructorRepository repository, JwtHelper jwt, UsersRepo userrepository)
        {
            _repository = repository;
            _jwt = jwt;
            _userRepository = userrepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var email = GetCurrentUserEmail();
            if (email != null)
            {
                var user = _repository.GetUserByEmail(email);
                if (user != null)
                {
                    var instructor = _repository.GetById(user.UserID);
                    if (instructor != null)
                    {
                        _isManager = instructor.Branches.Any();
                        ViewBag.IsManager = _isManager;
                        ViewBag.ManagedBranches = _isManager ? instructor.Branches : new List<Branch>();
                    }
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManagedBranch()
        {
            if (!_isManager)
                return RedirectToAction("Index");

            var instructor = GetCurrentInstructor();
            var managedBranch = instructor.Branches.FirstOrDefault();

            if (managedBranch == null)
                return View("NoBranch");
    
            var vm = new DisplayBranchVM
            {
                BranchID = managedBranch.BranchID,
                BranchName = managedBranch.BranchName,
                Governate = managedBranch.Governate,
                City = managedBranch.City,
                Street = managedBranch.Street,
                Tel = managedBranch.Tel,
            };

            return View(vm);
        }

        public IActionResult Dashboard()
        {
            var instructor = GetCurrentInstructor();
            var dashboardData = _repository.GetDashboardData(instructor.InsID);
            return View(dashboardData);
           
        }

    

        public IActionResult ReportViewer()
        {
            
            var reports = new List<SelectListItem>
        {
            new SelectListItem { Value = "Exam Report/ExamDetailsReport", Text = "Exam Details" },
            new SelectListItem { Value = "StudentResultsReport/StudentResultsReport", Text = "Student Results" },
            new SelectListItem { Value = "Exam With out correct Answer/ExamDetailswithoutAnswerReport", Text = "Exam Questions" },
                      //new SelectListItem { Value = "ExamQuestions/QuestionsAnswerReport", Text = "Questions Correct Answer" }
        };

            ViewBag.Reports = reports;
            return View();
        }



        public IActionResult RedirectToReport(string selectedReport)
        {
            if (string.IsNullOrEmpty(selectedReport))
                return RedirectToAction("ReportViewer");

            var reportUrl = $"http://localhost/ReportServer?/{selectedReport}&rs:Command=Render";
            return Redirect(reportUrl);
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
            var instructor = GetCurrentInstructor();
            var tracks = _repository.GetInstructorTracks(instructor.InsID);
            return View(tracks);
        }

        public IActionResult Courses(int trackId)
        {
            var instructor = GetCurrentInstructor();
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
            var instructor = GetCurrentInstructor();
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
            var instructor = GetCurrentInstructor();
            var model = new GenerateExamViewModel
            {
                Courses = _repository.GetCourseSelectList(instructor.InsID)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult GenerateExam(GenerateExamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var instructor = GetCurrentInstructor();
                model.Courses = _repository.GetCourseSelectList(instructor.InsID);
                return View(model);
            }

            var result = _repository.GenerateExam(model);
            return View("GeneratedExamDetails", result);
        }

        public IActionResult InstructorCoursesSummary()
        {
            var instructor = GetCurrentInstructor();
            var courses = _repository.GetInstructorCoursesSummary(instructor.InsID);
            return View(courses);
        }

        public IActionResult CourseStudents(int courseId, int? trackId)
        {
            var students = _repository.GetCourseStudents(courseId, trackId);
            ViewBag.CourseName = _repository.GetCourseName(courseId);
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
            var exams = _repository.GetCourseExams(courseId);
            ViewBag.CourseName = _repository.GetCourseName(courseId);
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

        public IActionResult AddQuestion()
        {
            var instructor = GetCurrentInstructor();
            ViewBag.Courses = _repository.GetCourseSelectList(instructor.InsID);
            return View();
        }

        [HttpPost]
        public IActionResult AddQuestion(QuestionViewModel model)
        {
            var validationResults = model.Validate(new ValidationContext(model));
            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    ModelState.AddModelError(memberName, result.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.Points = model.Type == "T/F" ? 1 : 2;

                    int questionId = _repository.InsertQuestion(
                        model.QText,
                        model.Type,
                        model.Points,
                        model.CrsID,
                        model.CorrectOptNum
                    );

                    if (questionId > 0)
                    {
                        string opt1 = model.Type == "T/F" ? "True" : model.OptText1;
                        string opt2 = model.Type == "T/F" ? "False" : model.OptText2;
                        string opt3 = model.Type == "T/F" ? null : model.OptText3;
                        string opt4 = model.Type == "T/F" ? null : model.OptText4;

                        int optionResult = _repository.InsertOptionsUsingSP(
                            questionId,
                            opt1, opt2, opt3, opt4
                        );

                        if (optionResult > 0)
                        {
                            TempData["Success"] = "Question and options added successfully!";
                            return RedirectToAction("AddQuestion");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            var instructor = GetCurrentInstructor();
            ViewBag.Courses = _repository.GetCourseSelectList(instructor.InsID);
            return View(model);
        }

        #region Helper Methods
        private string GetCurrentUserEmail()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return null;

            var principal = _jwt.ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.Email)?.Value;
        }

        private Instructor GetCurrentInstructor()
        {
            var email = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException();

            var user = _repository.GetUserByEmail(email);
            if (user == null)
                throw new UnauthorizedAccessException();

            return _repository.GetById(user.UserID) ?? throw new UnauthorizedAccessException();
        }
        #endregion
    }
}