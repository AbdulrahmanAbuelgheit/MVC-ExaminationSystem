using AutoMapper;
using ExaminationSystemMVC.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class ManagerTrackController : Controller
    {
        private readonly UnitOfWork _unit;
        private readonly IMapper _map;
        private readonly int _managerBranchId;

        public ManagerTrackController(UnitOfWork unit, IMapper map, IHttpContextAccessor httpContextAccessor)
        {
            _unit = unit;
            _map = map;

            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = _unit.InstructorRepo.GetById(int.Parse(userId));

            _managerBranchId = _unit.BranchRepo.GetBranchIdByManagerId(manager.InsID);
        }

        public IActionResult Index()
        {
            var branch = _unit.BranchRepo.GetBranchByIdWithTracks(_managerBranchId);
            if (branch == null)
            {
                return NotFound();
            }

            var tracksVM = _map.Map<List<DisplayTrackVM>>(branch.Tracks);
            return View(tracksVM);
        }

        public IActionResult Details(int id, int? intakeYear, string sortOrder, int trackId)
        {
            try
            {
                var track = _unit.TrackRepo.GetById(id);
                if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
                    return NotFound();

                var students = _unit.StudentRepo.GetByTrackAndBranch(id, _managerBranchId)
                    .Select(s => new DisplayStudentVM
                    {
                        StdID = s.StdID,
                        Name = $"{s.Std.FirstName} {s.Std.LastName}",
                        Email = s.Std.Email,
                        IntakeYear = s.IntakeYear,
                        TrackID = trackId,
                        IsActive = "Active" 
                    }).ToList();

                if (intakeYear.HasValue)
                    students = students.Where(s => s.IntakeYear == intakeYear.Value).ToList();

                if (!string.IsNullOrEmpty(sortOrder))
                    students = sortOrder.ToLower() == "asc"
                        ? students.OrderBy(s => s.IntakeYear).ToList()
                        : students.OrderByDescending(s => s.IntakeYear).ToList();


                var trackVM = new DisplayTrackVM
                {
                    TrackID = track.TrackID,
                    TrackName = track.TrackName,
                    Duration = track.Duration,
                    Capacity = track.Capacity,
                    SupervisorID = track.SupervisorID,
                    SupervisorName = track.Supervisor != null
                        ? $"{track.Supervisor.Ins.FirstName} {track.Supervisor.Ins.LastName}"
                        : null,
                    Students = students,
                };

                ViewBag.AvailableStudents = _unit.StudentRepo
                    .GetByBranch(_managerBranchId)
                    .Where(s => s.TrackID != id)
                    .Select(s => new {
                        s.StdID,
                        Name = $"{s.Std.FirstName} {s.Std.LastName}",
                        s.Std.Email
                    }).ToList();
                var trackWithCourses = _unit.TrackRepo.GetTrackWithCourses(id);
                trackVM.Courses = _map.Map<List<DisplayCourseVM>>(trackWithCourses?.Crs ?? new List<Course>());

                var trackWithInstructors = _unit.TrackRepo.GetTrackWithInstructors(id);
                trackVM.Instructors = _map.Map<List<DisplayInstructorVM>>(trackWithInstructors?.Ins ?? new List<Instructor>());

                return View(trackVM);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignStudent(int trackId, int studentId)
        {
            var track = _unit.TrackRepo.GetById(trackId);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
            {
                return NotFound();
            }

            var student = _unit.StudentRepo.GetById(studentId);
            if (student == null || student.BranchID != _managerBranchId)
            {
                return NotFound();
            }

            student.TrackID = trackId;
            _unit.StudentRepo.Update(student);
            _unit.Save();

            return RedirectToAction("Details", new { id = trackId });
        }
        [HttpGet]
        public IActionResult Create()
        {
            var branchInstructors = _unit.BranchRepo.GetById(_managerBranchId)?.Ins ?? new List<Instructor>();

            var availableInstructors = branchInstructors
                .Select(i => new {
                    i.InsID,
                    FullName = $"{i.Ins.FirstName} {i.Ins.LastName}"
                })
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
                var branchInstructors = _unit.BranchRepo.GetById(_managerBranchId)?.Ins ?? new List<Instructor>();
                vm.AvailableInstructors = new SelectList(
                    branchInstructors.Select(i => new {
                        i.InsID,
                        FullName = $"{i.Ins.FirstName} {i.Ins.LastName}"
                    }),
                    "InsID",
                    "FullName"
                );
                return View(vm);
            }

            var track = new Track
            {
                TrackName = vm.TrackName,
                Duration = vm.Duration,
                Capacity = vm.Capacity,
                SupervisorID = vm.SupervisorID,
                Branches = new List<Branch> { _unit.BranchRepo.GetById(_managerBranchId) }
            };

            _unit.TrackRepo.Add(track);
            _unit.Save();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult AddCourse(int id)
        {
            var track = _unit.TrackRepo.GetTrackWithDetails(id);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
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
        [ValidateAntiForgeryToken]
        public IActionResult AddCourse(AddCourseToTrackVM vm)
        {
            var track = _unit.TrackRepo.GetTrackWithCourses(vm.TrackID);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
                return NotFound();

            if (ModelState.IsValid)
            {
                var course = _unit.CourseRepo.GetById(vm.CourseID);
                if (course == null)
                    return NotFound();

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
            var track = _unit.TrackRepo.GetTrackWithDetails(trackId);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
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
        public IActionResult AddInstructor(int id) 
        {
            var instructors = _unit.InstructorRepo.GetInstructorsByBranchId(_managerBranchId);

            var track = _unit.TrackRepo.GetTrackWithInstructors(id);

            var availableInstructors = instructors
                .Where(i => !track.Ins.Any(ti => ti.InsID == i.InsID))
                .Select(i => new SelectListItem
                {
                    Value = i.InsID.ToString(),
                    Text = $"{i.InsName} ({i.Email})"
                })
                .ToList();

            var vm = new AddInstructorToTrackVM
            {
                TrackID = id,
                AvailableInstructors = new SelectList(availableInstructors, "Value", "Text")
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeInstructorSupervisor(int trackId, int insId)
        {
            var track = _unit.TrackRepo.GetById(trackId);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
                return NotFound();

            var instructor = _unit.InstructorRepo.GetById(insId);
            if (instructor == null || !_unit.InstructorRepo.IsInstructorInBranch(insId, _managerBranchId))
                return NotFound();

            track.SupervisorID = insId;
            _unit.TrackRepo.Update(track);
            _unit.Save();

            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveInstructor(int trackId, int insId)
        {
            var track = _unit.TrackRepo.GetTrackWithInstructors(trackId);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
                return NotFound();

            var instructor = track.Ins.FirstOrDefault(i => i.InsID == insId);
            if (instructor != null)
            {
                track.Ins.Remove(instructor);
                _unit.TrackRepo.Update(track);
                _unit.Save();
            }

            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCourse(int trackId, int courseId)
        {
            var track = _unit.TrackRepo.GetTrackWithCourses(trackId);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
                return NotFound();

            var course = track.Crs.FirstOrDefault(c => c.CrsID == courseId);
            if (course != null)
            {
                track.Crs.Remove(course);
                _unit.TrackRepo.Update(track);
                _unit.Save();
            }

            return RedirectToAction("Details", new { id = trackId });
        }
        [HttpGet]
        public IActionResult AssignInstructorToCourse(int courseId, int trackId)
        {
            var course = _unit.CourseRepo.GetById(courseId);
            if (course == null) return NotFound();

            var instructors = _unit.InstructorRepo.GetInstructorsByBranch(_managerBranchId)
                .Where(i => !course.Ins.Any(ci => ci.InsID == i.InsID))
                .ToList();

            var vm = new AssignInstructorToCourseVM
            {
                CourseID = courseId,
                TrackID = trackId,
                CourseName = course.CrsName,
                Instructors = _map.Map<List<DisplayInstructorVM>>(instructors)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AssignInstructorToCourse(AssignInstructorToCourseVM vm)
        {
            var course = _unit.CourseRepo.GetCourseWithInstructors(vm.CourseID);
            var instructor = _unit.InstructorRepo.GetById(vm.SelectedInstructorId);

            if (course == null || instructor == null ||
                !_unit.InstructorRepo.IsInstructorInBranch(vm.SelectedInstructorId, _managerBranchId))
            {
                return NotFound();
            }

            if (!course.Ins.Any(i => i.InsID == vm.SelectedInstructorId))
            {
                course.Ins.Add(instructor);
                _unit.CourseRepo.Update(course);
                _unit.Save();
            }

            return RedirectToAction("CourseDetails", new
            {
                id = vm.CourseID,
                trackId = vm.TrackID
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveInstructorFromCourse(int courseId, int instructorId, int trackId)
        {
            var course = _unit.CourseRepo.GetCourseWithInstructors(courseId);
            if (course == null) return NotFound();

            var instructor = course.Ins.FirstOrDefault(i => i.InsID == instructorId);
            if (instructor != null)
            {
                course.Ins.Remove(instructor);
                _unit.CourseRepo.Update(course);
                _unit.Save();
            }

            return RedirectToAction("CourseDetails", new { id = courseId, trackId });
        }

        [HttpGet]
        public IActionResult CourseDetails(int id, int trackId)
        {

            var course = _unit.CourseRepo.GetCourseWithInstructors(id);
            if (course == null) return NotFound();

            var vm = new CourseDetailsVM
            {
                CourseID = course.CrsID,
                TrackID = trackId,
                CourseName = course.CrsName,
                Description = course.Description,
                Instructors = _map.Map<List<DisplayInstructorVM>>(course.Ins)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInstructor(AddInstructorToTrackVM vm)
        {
            var track = _unit.TrackRepo.GetTrackWithInstructors(vm.TrackID);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
                return NotFound();

            if (ModelState.IsValid)
            {
                var instructor = _unit.InstructorRepo.GetById(vm.InsID);
                if (instructor == null || !_unit.InstructorRepo.IsInstructorInBranch(vm.InsID, _managerBranchId))
                    return NotFound();

                if (!track.Ins.Any(i => i.InsID == vm.InsID))
                {
                    track.Ins.Add(instructor);
                    _unit.TrackRepo.Update(track);
                    _unit.Save();
                    return RedirectToAction("Details", new { id = vm.TrackID });
                }

                ModelState.AddModelError("", "Instructor is already assigned to this track");
            }

            vm.AvailableInstructors = GetAvailableInstructors(vm.TrackID);
            return View(vm);
        }

        private SelectList GetAvailableInstructors(int trackId)
        {
            var instructors = _unit.InstructorRepo.GetInstructorsByBranch(_managerBranchId);
            var track = _unit.TrackRepo.GetTrackWithInstructors(trackId);

            var availableInstructors = instructors
                .Where(i => !track.Ins.Any(ti => ti.InsID == i.InsID))
                .Select(i => new SelectListItem
                {
                    Value = i.InsID.ToString(),
                    Text = $"{i.InsName} ({i.Email})"
                });

            return new SelectList(availableInstructors, "Value", "Text");
        }

        [HttpGet]
        public IActionResult EditTrack(int id)
        {
            var track = _unit.TrackRepo.GetByIdWithBranches(id);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
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
        [ValidateAntiForgeryToken]
        public IActionResult EditTrack(EditTrackVM vm)
        {
            var track = _unit.TrackRepo.GetByIdWithBranches(vm.TrackID);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            track.TrackName = vm.TrackName;
            track.Duration = vm.Duration;
            track.Capacity = vm.Capacity;

            _unit.TrackRepo.Update(track);
            _unit.Save();

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTrack(int id)
        {
            var track = _unit.TrackRepo.GetByIdWithBranches(id);
            if (track == null || !track.Branches.Any(b => b.BranchID == _managerBranchId))
            {
                return NotFound();
            }

            try
            {
                var branch = _unit.BranchRepo.GetById(_managerBranchId);
                if (branch != null)
                {
                  
                    branch.Tracks.Remove(track);
                    _unit.BranchRepo.Update(branch);

                    track.Branches.Remove(branch);
                    _unit.TrackRepo.Update(track);

                    _unit.Save();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting track: {ex.Message}");
                TempData["ErrorMessage"] = "Error occurred while removing the track from your branch.";
                return RedirectToAction("Index");
            }
        }
        #region Student Management Actions
        [HttpGet]
        public IActionResult ViewStudent(int id)
        {
            var student = _unit.StudentRepo.GetById(id);
            if (student == null || student.BranchID != _managerBranchId)
                return NotFound();

            return View(_map.Map<DisplayStudentVM>(student));
        }



        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            var student = _unit.StudentRepo.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            var mappedStudent = _map.Map<EditStudentVM>(student);
            ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", mappedStudent.BranchID);
            ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", mappedStudent.TrackID);

            return View(mappedStudent);
        }

        [HttpPost]
        public IActionResult EditStudent(EditStudentVM studentVM)
        {
            if (!ModelState.IsValid)
            {

                var student = _unit.StudentRepo.GetById(studentVM.StdID);
                if (student != null)
                {
                    studentVM.FullName = student.Std.FirstName + " " + student.Std.LastName;
                    studentVM.Email = student.Std.Email;
                }

                ViewBag.Branches = new SelectList(_unit.BranchRepo.GetAll(), "BranchID", "BranchName", studentVM.BranchID);
                ViewBag.Tracks = new SelectList(_unit.TrackRepo.GetAll(), "TrackID", "TrackName", studentVM.TrackID);
                return View(studentVM);
            }
            var mappedStudent = _map.Map<Student>(studentVM);
            _unit.StudentRepo.Update(mappedStudent);
            _unit.Save();
            return RedirectToAction("Index","ManagerTrack");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStudent(int id)
        {
            var student = _unit.StudentRepo.GetById(id);
            if (student == null || student.BranchID != _managerBranchId)
                return NotFound();

            var trackId = student.TrackID; 
            _unit.StudentRepo.Delete(id); 
            _unit.Save();

            return RedirectToAction("Details", new { id = trackId });
        }
        #endregion
    }
}