using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ExaminationSystemMVC.DTOs.AuthDTO.Auth;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminInstructorController : Controller
    {

        private readonly UnitOfWork _unit;
        private readonly IMapper _map;

        public AdminInstructorController(UnitOfWork unit, IMapper map)
        {
            _unit = unit;
            _map = map;
        }


        public IActionResult Index()
        { 
            var instructors = _unit.InstructorRepo.GetAllWithBranchAndTrack();


            return View(instructors);
        }

        public IActionResult AllDetails(int id)
        {
            var instructors = _unit.InstructorRepo.GetInstructorEithBranchAndTrackById(id);

            return View(instructors);
        }

        public IActionResult Edit(int id)
        {
            var instructor = _unit.InstructorRepo.GetInstructorForEdit(id);
            return View(instructor);
        }

        [HttpPost]
        public IActionResult Edit(EditInstructorViewModel instructor)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fill all fields according to the isntructions");

                return View(instructor);
            }

            _unit.InstructorRepo.EditInstructor(instructor);

            return RedirectToAction("Index");

        }

        public IActionResult Delete(int id)
        {
            _unit.InstructorRepo.DeleteInstructor(id);
            return RedirectToAction("Index");

        }

        public IActionResult AddInstructor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddInstructor(RegisterInstructorVM instructor)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fill all fields according to the isntructions");
                return View(instructor);
            }

            var existingUser = _unit.UserRepo.GetByEmail(instructor.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(instructor);
            }

            var user = new User
            {
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                Email = instructor.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(instructor.Password),
                Birthdate = instructor.Birthdate,
                Street = instructor.Street,
                Governate = instructor.Governate,
                City = instructor.City,
                role = "Instructor"
            };

            _unit.UserRepo.Add(user);
            _unit.Save();

            _unit.InstructorRepo.AddInstructor(new Instructor
            {
                Salary = instructor.Salary,
                InsID = user.UserID,
            });
            _unit.Save();

            return RedirectToAction("Index");
        }
    }
}
