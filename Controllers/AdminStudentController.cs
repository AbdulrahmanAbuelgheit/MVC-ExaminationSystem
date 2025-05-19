using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static ExaminationSystemMVC.DTOs.AuthDTO.Auth;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminStudentController : Controller
    {
        IMapper _map;
        UnitOfWork _unit;
        public AdminStudentController(IMapper map, UnitOfWork unit)
        {
            _map = map;
            _unit = unit;
        }
        public IActionResult Index()
        {
            var students = _unit.StudentRepo.GetAllWithBranchAndTrack();
            var mappedStudents = _map.Map<List<DisplayStudentVM>>(students);
            return View(mappedStudents);
        }

        [HttpGet]
        public IActionResult GetByID(int id)
        {
            var student = _unit.StudentRepo.GetByIdWithBranchAndTrack(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult RegisterStudent() 
        {
             ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName");
             ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName");
             return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(RegisterUserVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName");
                ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName");
                return View();
            }

            var existingUser = _unit.UserRepo.GetByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                City = model.City,
                Governate = model.Governate,
                Street = model.Street,
                Birthdate = model.Birthdate,
                role = "Student"
            };

            _unit.UserRepo.Add(user);
            _unit.Save();

            _unit.StudentRepo.Add(new Student
            {
                StdID = user.UserID,
                BranchID = model.BranchID,
                TrackID = model.TrackID,
                IntakeYear = model.IntakeYear
            });

            var track = _unit.TrackRepo.GetTrackWithCourses(model.TrackID);
            var newRelations = new List<Student_Course>();
            foreach (var course in track.Crs)
            {
                 newRelations.Add(new Student_Course
                 {
                     StdID = user.UserID,
                     CrsID = course.CrsID
                 });
            }
                
            _unit.StudentCourseRepo.AddRange(newRelations);
            _unit.Save();

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _unit.StudentRepo.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            var mappedStudent = _map.Map<EditStudentVM>(student);
            ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", mappedStudent.BranchID);
            ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", mappedStudent.TrackID);

            return View(mappedStudent);
        }

        [HttpPost]
        public IActionResult Edit(EditStudentVM studentVM)
        {
            if (!ModelState.IsValid)
            {

                var student = _unit.StudentRepo.GetById(studentVM.StdID);
                if (student != null)
                {
                    studentVM.FullName = student.Std.FirstName + " " + student.Std.LastName;
                    studentVM.Email = student.Std.Email;
                }

                ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", studentVM.BranchID);
                ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", studentVM.TrackID);
                return View(studentVM);
            }
            var mappedStudent = _map.Map<Student>(studentVM);
            _unit.StudentRepo.Update(mappedStudent);
            _unit.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var student = _unit.StudentRepo.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            _unit.StudentRepo.SoftDelete(id);
            _unit.Save();
            return RedirectToAction("Index");
        }
    }
}
