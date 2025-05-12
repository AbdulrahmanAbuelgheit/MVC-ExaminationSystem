using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ExaminationSystemMVC.Models;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class ManagerController : ManagerBaseController
    {
        private readonly IMapper _map;
        private readonly UnitOfWork _unit;
        private readonly int _managerBranchId;

        public ManagerController(UnitOfWork unit, IMapper map, IHttpContextAccessor httpContextAccessor)
        {
            _map = map;
            _unit = unit;

            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int managerId))
            {
                // Alternative way to get branch ID if not directly on Instructor
                _managerBranchId = _unit.BranchRepo.GetBranchIdByManagerId(managerId);
            }
            else
            {
                _managerBranchId = 0;
            }
        }

        public IActionResult Index()
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(_managerBranchId);
            if (branch == null)
            {
                return NotFound();
            }
           

            var branchDTO = _map.Map<DisplayBranchVM>(branch);
            return View(branchDTO);
        }

        public IActionResult Details()
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(_managerBranchId);
            if (branch == null)
            {
                return NotFound();
            }

            var branchDTO = _map.Map<DisplayBranchVM>(branch);
            return View(branchDTO);
        }

        [HttpGet]
        public IActionResult AddTrack()
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(_managerBranchId);
            if (branch == null)
                return NotFound();

            var availableTracks = _unit.TrackRepo.GetAll()
                .Where(t => !branch.Tracks.Any(bt => bt.TrackID == t.TrackID)).ToList();

            var vm = new AddTrackToBranchVM
            {
                BranchID = _managerBranchId,
            };

            vm.AvailableTracks = new SelectList(availableTracks, "TrackID", "TrackName");
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddTrack(AddTrackToBranchVM vm)
        {
            if (vm.BranchID != _managerBranchId)
            {
                return Forbid();
            }

            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(vm.BranchID);
            if (branch == null)
                return NotFound();

            var track = _unit.TrackRepo.GetById(vm.TrackID);
            if (track == null)
                return NotFound();

            branch.Tracks.Add(track);
            _unit.BranchRepo.Update(branch);
            _unit.Save();
            return RedirectToAction("Index", "ManagerTrack");
        }

        [HttpGet]
        public IActionResult RemoveTrack(int trackId)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(_managerBranchId);
            if (branch == null)
                return NotFound();

            var track = _unit.TrackRepo.GetById(trackId);
            if (track == null)
                return NotFound();

            branch.Tracks.Remove(track);
            _unit.BranchRepo.Update(branch);
            _unit.Save();
            return RedirectToAction("Details");
        }

        public IActionResult Edit()
        {
            var branch = _unit.BranchRepo.GetById(_managerBranchId);
            if (branch == null)
            {
                return NotFound();
            }

            var instructors = _unit.InstructorRepo.GetAllInstructors()
               .Select(i => new
               {
                   InsID = i.InsID,
                   Name = $"{i.Ins.FirstName} {i.Ins.LastName}"
               })
               .ToList();

            ViewBag.Instructors = new SelectList(instructors, "InsID", "Name", branch.ManagerID);
            var branchDTO = _map.Map<UpdateBranchVM>(branch);
            return View(branchDTO);
        }

        [HttpPost]
        public IActionResult Edit(UpdateBranchVM vm)
        {
            if (vm.BranchID != _managerBranchId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var branch = _unit.BranchRepo.GetById(vm.BranchID.Value);
                if (branch == null)
                {
                    return NotFound();
                }

                _map.Map(vm, branch);
                _unit.BranchRepo.Update(branch);
                _unit.Save();
                return RedirectToAction("Details");
            }

            var instructors = _unit.InstructorRepo.GetAllInstructors()
               .Select(i => new
               {
                   InsID = i.InsID,
                   Name = $"{i.Ins.FirstName} {i.Ins.LastName}"
               })
               .ToList();

            ViewBag.Instructors = new SelectList(instructors, "InsID", "Name", vm.ManagerID);
            return View(vm);
        }
    }
}
