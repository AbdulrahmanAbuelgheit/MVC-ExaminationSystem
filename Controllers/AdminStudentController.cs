using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Create()
        {
            ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName");
            ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName");

            var studentIds = _unit.StudentRepo.GetAll().Select(s => s.StdID);
            var users = _unit.UserRepo.GetAll()
                              .Where(user => !studentIds.Contains(user.UserID))
                              .Select(user => new
                                        {
                                               UserID = user.UserID,
                                               FullName = user.FirstName + " " + user.LastName
                                           }).ToList();

            ViewBag.users = new SelectList(users, "UserID", "FullName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateStudentVM student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName");
                ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName");
                var studentIds = _unit.StudentRepo.GetAll().Select(s => s.StdID);
                var users = _unit.UserRepo.GetAll()
                                  .Where(user => !studentIds.Contains(user.UserID))
                                  .Select(user => new
                                  {
                                      UserID = user.UserID,
                                      FullName = user.FirstName + " " + user.LastName
                                  }).ToList();

                ViewBag.users = new SelectList(users, "UserID", "FullName");
                return View(student);
            }

            var mappedStudent = _map.Map<Student>(student);
            _unit.StudentRepo.Add(mappedStudent);

            var user = _unit.UserRepo.GetById(student.StdID);
            user.role = "Student";
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
            var mappedStudent = _map.Map<UpdateStudentVM>(student);
            ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", mappedStudent.BranchID);
            ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", mappedStudent.TrackID);

            return View(mappedStudent);
        }

        [HttpPost]
        public IActionResult Edit(UpdateStudentVM studentVM)
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
