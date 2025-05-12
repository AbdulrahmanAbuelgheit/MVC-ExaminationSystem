using AutoMapper;
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
            var branchesDTO = _map.Map<List<DisplayBranchVM>>(branches);
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
        public  IActionResult Create(CreateBranchVM vm)
        {
            if (ModelState.IsValid)
            {
                var branch = _map.Map<Branch>(vm);
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
            return View(vm);
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
            var branchDTO = _map.Map<UpdateBranchVM>(branch);
            return View(branchDTO);
        }

        [HttpPost]
        public IActionResult Edit(UpdateBranchVM vm)
        {
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
                return RedirectToAction("Index");
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

        [HttpGet]
        public IActionResult Details(int id)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(id);
            if (branch == null)
            {
                return NotFound();
            }
            var branchDTO = _map.Map<DisplayBranchVM>(branch);
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

            var vm = new AddTrackToBranchVM
            {
                BranchID = id,
            };

            vm.AvailableTracks = new SelectList(AvailableTracks, "TrackID", "TrackName");
            return View(vm);

        }
        [HttpPost]
        public IActionResult AddTrack(AddTrackToBranchVM vm)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(vm.BranchID);
            if (branch == null)
                return NotFound();

            var track = _unit.TrackRepo.GetById(vm.TrackID);
            if (track == null)
                return NotFound();

            branch.Tracks.Add(track);
            _unit.BranchRepo.Update(branch);
            _unit.Save();
            return RedirectToAction("Details", new { id = vm.BranchID });
        }

        [HttpGet]
        public IActionResult RemoveTrack(int branchId, int trackId)
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(branchId);
            if (branch == null)
                return NotFound();
            var track = _unit.TrackRepo.GetById(trackId);
            if (track == null)
                return NotFound();
            branch.Tracks.Remove(track);
            _unit.BranchRepo.Update(branch);
            _unit.Save();
            return RedirectToAction("Details", new { id = branchId });
        }
    }
}
