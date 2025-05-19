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
            var branchVM = _map.Map<DisplayBranchVM>(branch);

            // Filter instructors per track in this branch
            branchVM.Tracks = branch.Tracks.Select(track => new DisplayTrackWithInstructorsVM
            {
                TrackID = track.TrackID,
                TrackName = track.TrackName,
                Duration = track.Duration,
                Capacity = track.Capacity,
                Supervisor = track.Supervisor,
                Instructors = track.Ins
                    .Where(i => branch.Ins.Any(bi => bi.InsID == i.InsID)) // Keep only those in both branch + track
                    .ToList()
            }).ToList();

            // Optional: eager load instructors and their tracks
            foreach (var track in branch.Tracks)
            {
                track.Ins = track.Ins
                    .Where(ti => branch.Ins.Any(bi => bi.InsID == ti.InsID))
                    .ToList();
            }

            return View(branchVM);
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
            if (!_unit.BranchRepo.SafeToDelete(trackId, branchId))
            {
                TempData["Error"] = "Track cannot be deleted because it has assigned instructors.";
                return RedirectToAction("Details", new {id = branchId});
            }

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

        [HttpGet]
        public IActionResult AddInstructor(int branchId, int trackId)
        {
            var branch = _unit.BranchRepo.GetBranchWithInstructors(branchId);
            if (branch == null)
            {
                return NotFound();
            }

            var availableInstructors = _unit.InstructorRepo.GetAllInstructors()
                .Where(i => !branch.Ins.Any(bi => bi.InsID == i.InsID))
                .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            var vm = new AddInstructorToTrackVM
            {
                TrackID = trackId,
                BranchID = branchId

            };
            ViewBag.InstructorsList = new SelectList(availableInstructors, "InsID", "FullName");
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddInstructor(AddInstructorToTrackVM vm)
        {

            var track = _unit.TrackRepo.GetTrackWithInstructors(vm.TrackID);
            if (track == null)
                return NotFound();

            var branch = _unit.BranchRepo.GetBranchWithInstructors(vm.BranchID);
            if (branch == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var instructor = _unit.InstructorRepo.GetById(vm.InsID);
                if (instructor == null)
                    return NotFound();

                track.Ins.Add(instructor);
                branch.Ins.Add(instructor);

                _unit.TrackRepo.Update(track);
                _unit.BranchRepo.Update(branch);
                _unit.Save();
                return RedirectToAction("Details", new { id = vm.BranchID });
            }


            var availableInstructors = _unit.InstructorRepo.GetAllInstructors()
                .Where(i => !track.Ins.Any(ti => ti.InsID == i.InsID))
                .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            ViewBag.InstructorsList = new SelectList(availableInstructors, "InsID", "FullName");
            return View(vm);
        }


        [HttpGet]
        public IActionResult RemoveInstructor(int branchId, int trackId, int instructorId)
        {
            var branch = _unit.BranchRepo.GetBranchWithInstructors(branchId);
            if (branch == null)
                return NotFound();

            var track = branch.Tracks.FirstOrDefault(t => t.TrackID == trackId);
            if (track == null)
                return NotFound();



            var instructor = track.Ins.FirstOrDefault(i => i.InsID == instructorId);
            if (instructor == null)
                return NotFound();


            branch.Ins.Remove(instructor);
            _unit.BranchRepo.Update(branch);
            _unit.Save();

            return RedirectToAction("Details", new { id = branchId });
        }

        [HttpPost]
        public IActionResult MakeInstructorSupervisor(int branchId, int trackId, int instructorId)
        {
            var branch = _unit.BranchRepo.GetBranchWithTrackAndInstructors(branchId);
            if (branch == null)
                return NotFound();

            var instructor = branch.Ins.FirstOrDefault(i => i.InsID == instructorId);
            if (instructor == null)
                return NotFound("Instructor does not work in this branch.");

            var track = branch.Tracks.FirstOrDefault(t => t.TrackID == trackId);
            if (track == null)
                return NotFound("Track does not exist in branch.");

            if (!track.Ins.Any(i => i.InsID == instructorId))
                return BadRequest("Instructor is not assigned to this track in this branch.");

            if (track.SupervisorID == instructorId)
                return BadRequest("Instructor is already the supervisor.");

            track.SupervisorID = instructorId;

            _unit.TrackRepo.Update(track);
            _unit.Save();

            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpPost]
        public IActionResult AssignManager(int branchId, int InstructorId)
        {
            var branch = _unit.BranchRepo.GetBranchWithInstructors(branchId);
            if (branch == null)
                return NotFound();


            var instructor = branch.Ins.FirstOrDefault(i => i.InsID == InstructorId);

            if (instructor == null)
                return NotFound();

            branch.ManagerID = InstructorId;
            _unit.BranchRepo.Update(branch);
            _unit.Save();

            return RedirectToAction("Details", new { id = branchId });
        }


    }
}
