using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystemMVC.Controllers
{
    public class AdminController : Controller
    {
        private  UnitOfWork _unit;
        public AdminController(UnitOfWork unit)
        {
            _unit = unit;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            ViewBag.StudentsCount = _unit.StudentRepo.GetAll().Count();
            ViewBag.TracksCount = _unit.TrackRepo.GetAll().Count();
            ViewBag.BranchesCount = _unit.BranchRepo.GetAll().Count();
            ViewBag.InstructorsCount = _unit.InstructorRepo.GetAllInstructors().Count();
            ViewBag.CoursesCount = _unit.CourseRepo.GetAll().Count();
            ViewBag.UsersCount = _unit.UserRepo.GetAll().Count();

            return View();
        }

        [Route("Admin/Home")]
        public IActionResult AdminHome()
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            return View("~/Views/Home/Index.cshtml");
        }

    }
}
