﻿using AutoMapper;
using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.Models.JWT;
using ExaminationSystemMVC.Reposatories;
using ExaminationSystemMVC.UnitOfWorks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static ExaminationSystemMVC.DTOs.AuthDTO.Auth;

namespace ExaminationSystemMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UnitOfWork _unit;
        private readonly IMapper _map;
        private readonly JwtHelper _jwt;

        public AccountController(UnitOfWork unit, IMapper map, JwtHelper jwt)
        {
            _unit = unit;
            _map = map;
            _jwt = jwt;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

            var token = _jwt.GenerateToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(2)
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {

                var principal = _jwt.ValidateToken(token);

                if (principal != null)
                {
                    var roleClaim = principal.Claims.FirstOrDefault(s => s.Type == ClaimTypes.Role);

                    if (roleClaim != null)
                    {
                        var role = roleClaim.Value;
                        if (role == "Admin")
                            return RedirectToAction("AdminHome", "Admin");
                        else if (role == "Instructor")
                            return RedirectToAction("Index", "Instructor");
                        else if (role == "Student")
                            return RedirectToAction("Exams", "Student");
                    }
                    else

                        return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserVM model)
        {

            var user = (_unit.UserRepo).GetByEmail(model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            var token = _jwt.GenerateToken(user); 

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(2)
            });

            if (user.role == "Admin")
                return RedirectToAction("AdminHome", "Admin");
            else if (user.role == "Instructor")
                return RedirectToAction("Index", "Instructor");
            else if (user.role == "Student")
                return RedirectToAction("Exams", "Student");
            else
                return RedirectToAction("Index", "Home"); 
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");

        }

        public IActionResult AccessDenied()
        {
            return View();
        }


    }

}
