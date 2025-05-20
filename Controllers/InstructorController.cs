using ExaminationSystemMVC.Models.JWT;
using ExaminationSystemMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


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
            var email = GetCurrentUserEmail();
            if (email == null) return Unauthorized();

            var user = _repository.GetUserByEmail(email);
            if (user == null) return Unauthorized();

            var instructor = _repository.GetById(user.UserID);
            if (instructor == null) return NotFound();

            bool isManager = instructor.Branches.Any(); // Branches managed by this instructor

            ViewBag.IsManager = isManager;
            ViewBag.ManagedBranches = isManager ? instructor.Branches : new List<Branch>();

            return View();
        }
        public IActionResult ManagedBranch()
        {
            var email = GetCurrentUserEmail();
            if (email == null) return Unauthorized();

            var user = _userRepository.GetByEmail(email);
            if (user == null) return NotFound();

            var instructor = _repository.GetById(user.UserID);
            if (instructor == null) return NotFound();

            var managedBranch = instructor.Branches.FirstOrDefault(); // Get the branch where this instructor is the manager
            if (managedBranch == null)
                return View("NoBranch"); // Optional: create a view saying "You're not managing any branch"

            var vm = new DisplayBranchVM
            {
                BranchID = managedBranch.BranchID,
                BranchName = managedBranch.BranchName,
                Governate = managedBranch.Governate,
                City = managedBranch.City,
                Street = managedBranch.Street,
                Tel = managedBranch.Tel,

            };

            return View("ManagedBranch", vm);
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

        //public IActionResult ReportViewer()
        //{
        //    var reports = new List<SelectListItem>
        //        {
        //            new SelectListItem { Value = "Exam%20Report/ExamDetailsReport", Text = "Exam Details Report" },
        //            new SelectListItem { Value = "Exam%20Report/StudentResultsReport", Text = "Student Results Report" },
        //            // أضف تقارير أخرى هنا بنفس الشكل
        //        };

        //    ViewBag.Reports = reports;

        //    return View();
        //}


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


        public IActionResult AddQuestion()
        {
            var instructorId = GetCurrentUserIdFromDatabase();
            var courses = _repository.GetCourseSelectList(instructorId);
            ViewBag.Courses = courses;
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
                        else
                        {
                            ModelState.AddModelError("", "Failed to add question options");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to add question");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            var instructorId = GetCurrentUserIdFromDatabase();
            ViewBag.Courses = _repository.GetCourseSelectList(instructorId);
            return View(model);
        }

       

    }
}