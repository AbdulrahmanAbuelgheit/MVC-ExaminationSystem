using AutoMapper;
using ExaminationSystemMVC.DTOs.UserDTO;
using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystemMVC.Controllers
{
    public class UsersController : Controller
    {
        IMapper _map;
        UnitOfWork _unit;
        public UsersController(IMapper map, UnitOfWork unit)
        {
            _map = map;
            _unit = unit;
        }
        public IActionResult GetAll()
        {
            List<User> users = _unit.UserRepo.GetAll();
            var mappedUsers = _map.Map<List<DisplayUserDTO>>(users);
            return View(mappedUsers);
        }

    }
}
