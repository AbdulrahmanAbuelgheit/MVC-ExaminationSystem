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
        public IActionResult Dashboard()
        {
            ViewBag.StudentCount = _unit.StudentRepo.GetAll().Count();
            ViewBag.TrackCount = _unit.TrackRepo.GetAll().Count();
            ViewBag.BranchCount = _unit.BranchRepo.GetAll().Count();
            ViewBag.InstructorCount = _unit.InstructorRepo.GetAll().Count();
            ViewBag.CourseCount = _unit.CourseRepo.GetAll().Count();
            ViewBag.UserCount = _unit.UserRepo.GetAll().Count();

            return View();
        }
    }
}
