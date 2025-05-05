using AutoMapper;
using ExaminationSystemMVC.DTOs.AdminDTOs.BranchDTOs;
using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBranchController : Controller
    {
        IMapper _map;
        UnitOfWork _unit;
        public AdminBranchController(UnitOfWork unit, IMapper map)
        {
            _map = map;
            _unit = unit;
        }

        public UnitOfWork Unit { get; }
        public IMapper Map { get; }

        public IActionResult Index()
        {
            var branches = _unit.BranchRepo.GetAll().ToList();
            var branchesDTO = _map.Map<List<DisplayBranchDTO>>(branches);
            return View(branchesDTO);
        }

        public IActionResult Create()
        {
            var instructors = _unit.InstructorRepo.GetAllInstructors()
                .Select(i => new
                {
                    InsID = i.InsID,
                    Name = $"{i.Ins.FirstName} {i.Ins.LastName}"
                })
                .ToList();
            ViewBag.Instructors = new SelectList(instructors, "InsID", "Name");
            return View();
        }


        [HttpPost]
        public  IActionResult Create(CreateBranchDTO dto)
        {
            if (ModelState.IsValid)
            {
                var branch = _map.Map<Branch>(dto);
                _unit.BranchRepo.Add(branch);
                _unit.Save();
                return RedirectToAction("Index");
            }

            var instructors = _unit.InstructorRepo.GetAllInstructors()
               .Select(i => new
               {
                   InsID = i.InsID,
                   Name = $"{i.Ins.FirstName} {i.Ins.LastName}"
               })
               .ToList();
            ViewBag.Instructors = new SelectList(instructors, "InsID", "Name");
            return View(dto);
        }

        public IActionResult Edit(int id)
        {
            var branch = _unit.BranchRepo.GetById(id);
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
            var branchDTO = _map.Map<UpdateBranchDTO>(branch);
            return View(branchDTO);
        }

        [HttpPost]
        public IActionResult Edit(UpdateBranchDTO dto)
        {
            if (ModelState.IsValid)
            {
                var branch = _unit.BranchRepo.GetById(dto.BranchID);
                if (branch == null)
                {
                    return NotFound();
                }
                _map.Map(dto, branch);
                _unit.BranchRepo.Update(branch);
                _unit.Save();
                return RedirectToAction("Index");
            }

            var instructors = _unit.InstructorRepo.GetAllInstructors()
               .Select(i => new
               {
                   InsID = i.InsID,
                   Name = $"{i.Ins.FirstName} {i.Ins.LastName}"
               })
               .ToList();
            ViewBag.Instructors = new SelectList(instructors, "InsID", "Name", dto.ManagerID);
            return View(dto);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(id);
            if (branch == null)
            {
                return NotFound();
            }
            var branchDTO = _map.Map<DisplayBranchDTO>(branch);
            return View(branchDTO);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var branch = _unit.BranchRepo.GetById(id);
            if (branch == null)
            {
                return NotFound();
            }
            _unit.BranchRepo.Delete(id);
            _unit.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddTrack(int id)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(id);
            if (branch == null)
                return NotFound();

            var AvailableTracks = _unit.TrackRepo.GetAll()
                .Where(t => !branch.Tracks.Any(bt => bt.TrackID == t.TrackID)).ToList();

            var dto = new AddTrackToBranchDTO
            {
                BranchID = id,
            };

            dto.AvailableTracks = new SelectList(AvailableTracks, "TrackID", "TrackName");
            return View(dto);

        }
        [HttpPost]
        public IActionResult AddTrack(AddTrackToBranchDTO dto)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(dto.BranchID);
            if (branch == null)
                return NotFound();

            var track = _unit.TrackRepo.GetById(dto.TrackID);
            if (track == null)
                return NotFound();

            branch.Tracks.Add(track);
            _unit.BranchRepo.Update(branch);
            _unit.Save();
            return RedirectToAction("Details", new { id = dto.BranchID });
        }
    }
}
