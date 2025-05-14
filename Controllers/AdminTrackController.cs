using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTrackController : Controller
    {
        private UnitOfWork _unit;
        private IMapper _map;

        public AdminTrackController(UnitOfWork unit, IMapper map)
        {
            _unit = unit;
            _map = map;
        }
        public IActionResult Index()
        {
            var tracks = _unit.TrackRepo.GetAll().ToList();
            var tracksVM = _map.Map<List<DisplayTrackVM>>(tracks);
            return View(tracksVM);
        }

        public IActionResult Details(int id, int? intakeYear, string sortOrder)
        {
            var track = _unit.TrackRepo.GetTrackWithDetails(id);
            if (track == null)
            {
                return NotFound();
            }

            var trackVM = _map.Map<DisplayTrackVM>(track);

            if (intakeYear.HasValue)
                trackVM.Students = trackVM.Students.Where(s => s.IntakeYear == intakeYear.Value).ToList();

            if (!string.IsNullOrEmpty(sortOrder))
                trackVM.Students = sortOrder.ToLower() == "asc"
                    ? trackVM.Students.OrderBy(s => s.IntakeYear).ToList()
                    : trackVM.Students.OrderByDescending(s => s.IntakeYear).ToList();

            return View(trackVM);
        }

        [HttpGet]
        public IActionResult AddCourse(int id)
        {
            var track = _unit.TrackRepo.GetTrackWithCourses(id);
            if (track == null)
            {
                return NotFound();
            }

            var availableCourses = _unit.CourseRepo.GetAll()
                .Where(c => !track.Crs.Any(tc => tc.CrsID == c.CrsID))
                .ToList();

            if (!availableCourses.Any())
            {
                ModelState.AddModelError("", "No available courses to add.");
                return View("Error");
            }

            var addCourseToTrackVM = new AddCourseToTrackVM
            {
                TrackID = id,

            };

            ViewBag.AvailableCourses = new SelectList(availableCourses, "CrsID", "CrsName");

            return View(addCourseToTrackVM);
        }

        [HttpPost]
        public IActionResult AddCourse(AddCourseToTrackVM vm)
        {
            var track = _unit.TrackRepo.GetTrackWithCourses(vm.TrackID);

            if (ModelState.IsValid)
            {
                if (track == null)
                {
                    return NotFound();
                }

                var course = _unit.CourseRepo.GetById(vm.CourseID);
                if (course == null)
                {
                    return NotFound();
                }

                track.Crs.Add(course);
                _unit.TrackRepo.Update(track);
                _unit.Save();
                return RedirectToAction("Details", new { id = vm.TrackID });
            }

            var availableCourses = _unit.CourseRepo.GetAll()
                .Where(c => !track.Crs.Any(tc => tc.CrsID == c.CrsID))
                .ToList();
            ViewBag.AvailableCourses = new SelectList(availableCourses, "CrsID", "CrsName");
            return View(vm);
        }

        [HttpGet]
        public IActionResult DeleteCourse(int trackId, int courseId)
        {
            var track = _unit.TrackRepo.GetTrackWithCourses(trackId);
            if (track == null)
                return NotFound();

            var course = track.Crs.FirstOrDefault(c => c.CrsID == courseId);
            if (course == null)
                return NotFound();

            track.Crs.Remove(course);
            _unit.TrackRepo.Update(track);
            _unit.Save();
            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpGet]
        public IActionResult TransferStudent(int studentId, int currentTrackId)
        {
            var currentTrack = _unit.TrackRepo.GetTrackWithStudents(currentTrackId);
            if (currentTrack == null)
                return NotFound();

            var student = currentTrack.Students.FirstOrDefault(s => s.StdID == studentId);
            if (student == null)
                return NotFound();

            var availableTracks = _unit.TrackRepo.GetAll().Where(t => t.TrackID != currentTrackId);

            //ViewBag.CurrentTrackId = currentTrackId;
            ViewBag.Tracks = new SelectList(availableTracks, "TrackID", "TrackName");

            var model = new TransferStudentViewModel
            {
                StudentId = studentId,
                CurrentTrackId = currentTrackId,
                StudentName = student.Std.FirstName + ' ' + student.Std.LastName,
                AvailableTracks = new SelectList(availableTracks, "TrackID", "TrackName")
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult TransferStudent(TransferStudentViewModel model)
        {
            var student = _unit.StudentRepo.GetById(model.StudentId);
            if (student == null)
                return NotFound();

            student.TrackID = model.NewTrackId;
            _unit.StudentRepo.Update(student);
            _unit.Save();

            return RedirectToAction("Details", new { id = model.NewTrackId });
        }

        [HttpPost]
        public IActionResult MakeInstructorSupervisor(int trackId, int insID)
        {
            var track = _unit.TrackRepo.GetById(trackId);
            if (track == null) return NotFound();

            var instructor = _unit.InstructorRepo.GetById(insID);
            if (instructor == null) return NotFound();

            track.SupervisorID = insID;
            _unit.TrackRepo.Update(track);
            _unit.Save();

            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpGet]
        public IActionResult RemoveInstructor(int trackId, int insId)
        {
            var track = _unit.TrackRepo.GetTrackWithInstructors(trackId);
            if (track == null)
                return NotFound();

            var instructor = track.Ins.FirstOrDefault(i => i.InsID == insId);
            if (instructor == null)
                return NotFound();
            

            track.Ins.Remove(instructor);
            _unit.TrackRepo.Update(track);
            _unit.Save();
            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var availableInstructors = _unit.InstructorRepo.GetAllInstructors()
                .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            var createTrackVM = new CreateTrackVM
            {
                AvailableInstructors = new SelectList(availableInstructors, "InsID", "FullName")
            };

            return View(createTrackVM);
        }


        [HttpPost]
        public IActionResult Create(CreateTrackVM vm)
        {
            if (!ModelState.IsValid)
            {
                var availableInstructors = _unit.InstructorRepo.GetAllInstructors()
                    .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                    .ToList();
                vm.AvailableInstructors = new SelectList(availableInstructors, "InsID", "FullName");
                return View(vm);
            }

            var track = new Track
            {
                TrackName = vm.TrackName,
                Duration = vm.Duration,
                Capacity = vm.Capacity,
                SupervisorID = vm.SupervisorID
            };

            _unit.TrackRepo.Add(track);
            _unit.Save();

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult EditTrack(int id)
        {
            var track = _unit.TrackRepo.GetById(id);
            if (track == null)
            {
                return NotFound();
            }

            var editTrackVM = new EditTrackVM
            {
                TrackID = track.TrackID,
                TrackName = track.TrackName,
                Duration = track.Duration,
                Capacity = track.Capacity
            };

            return View(editTrackVM);
        }

        [HttpPost]
        public IActionResult EditTrack(EditTrackVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var track = _unit.TrackRepo.GetById(vm.TrackID);
            if (track == null)
            {
                return NotFound();
            }

            track.TrackName = vm.TrackName;
            track.Duration = vm.Duration;
            track.Capacity = vm.Capacity;

            _unit.TrackRepo.Update(track);
            _unit.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteTrack(int id)
        {
            var track = _unit.TrackRepo.GetById(id);
            if (track == null)
            {
                return NotFound();
            }

            _unit.TrackRepo.Delete(id);
            _unit.Save();

            return RedirectToAction("Index");
        }



    }
}
