using AutoMapper;
using ExaminationSystemMVC.Models;
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
            var tracksDTO = _map.Map<List<DisplayTrackDTO>>(tracks);
            return View(tracksDTO);
        }

        public IActionResult Details(int id, int? intakeYear, string sortOrder)
        {
            var track = _unit.TrackRepo.GetTrackWithDetails(id);
            if (track == null)
            {
                return NotFound();
            }

            var trackDTO = _map.Map<DisplayTrackDTO>(track);

            if (intakeYear.HasValue)
                trackDTO.Students = trackDTO.Students.Where(s => s.IntakeYear == intakeYear.Value).ToList();

            if (!string.IsNullOrEmpty(sortOrder))
                trackDTO.Students = sortOrder.ToLower() == "asc"
                    ? trackDTO.Students.OrderBy(s => s.IntakeYear).ToList()
                    : trackDTO.Students.OrderByDescending(s => s.IntakeYear).ToList();

            return View(trackDTO);
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

            var addCourseToTrackDTO = new AddCourseToTrackDTO
            {
                TrackID = id,

            };

            ViewBag.AvailableCourses = new SelectList(availableCourses, "CrsID", "CrsName");

            return View(addCourseToTrackDTO);
        }

        [HttpPost]
        public IActionResult AddCourse(AddCourseToTrackDTO dto)
        {
            var track = _unit.TrackRepo.GetTrackWithCourses(dto.TrackID);

            if (ModelState.IsValid)
            {
                if (track == null)
                {
                    return NotFound();
                }

                var course = _unit.CourseRepo.GetById(dto.CourseID);
                if (course == null)
                {
                    return NotFound();
                }

                track.Crs.Add(course);
                _unit.TrackRepo.Update(track);
                _unit.Save();
                return RedirectToAction("Details", new { id = dto.TrackID });
            }

            var availableCourses = _unit.CourseRepo.GetAll()
                .Where(c => !track.Crs.Any(tc => tc.CrsID == c.CrsID))
                .ToList();
            ViewBag.AvailableCourses = new SelectList(availableCourses, "CrsID", "CrsName");
            return View(dto);
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

        [HttpGet]
        public IActionResult AddInstructor(int id)
        {
            var track = _unit.TrackRepo.GetTrackWithInstructors(id);
            if (track == null)
            {
                return NotFound();
            }

            var availableInstructors = _unit.InstructorRepo.GetAll()
                .Where(i => !track.Ins.Any(ti => ti.InsID == i.InsID))
                .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            var addInstructorToTrackDTO = new AddInstructorToTrackDTO
            {
                TrackID = id,

            };
            ViewBag.InstructorsList = new SelectList(availableInstructors, "InsID", "FullName");
            return View(addInstructorToTrackDTO);
        }

        [HttpPost]
        public IActionResult AddInstructor(AddInstructorToTrackDTO dto)
        {

            var track = _unit.TrackRepo.GetTrackWithInstructors(dto.TrackID);
            if (track == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var instructor = _unit.InstructorRepo.GetById(dto.InsID);
                if (instructor == null)
                    return NotFound();

                track.Ins.Add(instructor);
                _unit.TrackRepo.Update(track);
                _unit.Save();
                return RedirectToAction("Details", new { id = dto.TrackID });
            }


            var availableInstructors = _unit.InstructorRepo.GetAll()
                .Where(i => !track.Ins.Any(ti => ti.InsID == i.InsID))
                .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            Console.WriteLine("InsID from form: " + dto.InsID);
            ViewBag.InstructorsList = new SelectList(availableInstructors, "InsID", "FullName");
            return View(dto);
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
    }
}
