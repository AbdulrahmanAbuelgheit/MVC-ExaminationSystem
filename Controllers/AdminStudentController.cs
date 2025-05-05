using AutoMapper;
using ExaminationSystemMVC.DTOs.AdminDTOs.StudentDTOs;
using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var mappedStudents = _map.Map<List<DisplayStudentDTO>>(students);
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
        public IActionResult Create(CreateStudentDTO student)
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
            var mappedStudent = _map.Map<UpdateStudentDTO>(student);
            ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", mappedStudent.BranchID);
            ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", mappedStudent.TrackID);

            return View(mappedStudent);
        }

        [HttpPost]
        public IActionResult Edit(UpdateStudentDTO studentDTO)
        {
            if (!ModelState.IsValid)
            {

                var student = _unit.StudentRepo.GetById(studentDTO.StdID);
                if (student != null)
                {
                    studentDTO.FullName = student.Std.FirstName + " " + student.Std.LastName;
                    studentDTO.Email = student.Std.Email;
                }

                ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", studentDTO.BranchID);
                ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", studentDTO.TrackID);
                return View(studentDTO);
            }
            var mappedStudent = _map.Map<Student>(studentDTO);
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
