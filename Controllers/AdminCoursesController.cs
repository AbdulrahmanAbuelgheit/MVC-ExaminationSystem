using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCoursesController : Controller
    {

        IMapper _map;
        UnitOfWork _unit;

        public AdminCoursesController(IMapper map, UnitOfWork unit)
        {
            _map = map;
            _unit = unit;
        }


        public IActionResult Index()
        {

            var courses = _unit.CourseRepo.GetAllWithTopics();

            return View(courses);
        }

        public IActionResult Details(int id)
        {
            var course = _unit.CourseRepo.GetByIdWithTopicsAndInstructors(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult AddInstructor(int id)
        {
            var track = _unit.CourseRepo.GetById(id);

            if (track == null)
            {
                return NotFound();
            }

            var Instructors = _unit.InstructorRepo.GetAllInstructors()
                .Select( i => new { i.InsID , FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            var AddInstructorToCourse = new AddInstrucotToCourseVM
            {
                CrsId = id,
            };

            ViewBag.InstructorsList = new SelectList(Instructors , "InsID" , "FullName");
            return View(AddInstructorToCourse);

        }

        [HttpPost]
        public IActionResult AddInstructor(AddInstrucotToCourseVM model)
        {

            var course = _unit.CourseRepo.GetById(model.CrsId);
            if (course == null)
            {
                return NotFound();
            }
            
            if(ModelState.IsValid)
            {
                var instructor = _unit.InstructorRepo.GetById(model.InsID);
                if (instructor == null)
                {
                    ModelState.AddModelError("InsID", "Instructor not found");
                    return View(model);
                }

                course.Ins.Add(instructor);
                _unit.CourseRepo.Update(course);
                _unit.Save();
                return RedirectToAction("Details" , new {id = model.CrsId});
            }

            var Instructors = _unit.InstructorRepo.GetAllInstructors()
                .Select(i => new { i.InsID, FullName = i.Ins.FirstName + " " + i.Ins.LastName })
                .ToList();

            ViewBag.InstructorsList = new SelectList(Instructors, "InsID", "FullName");
            return View(model);

        }

        public IActionResult DeleteInstructor(int courseId , int instructorId)
        {
            var course = _unit.CourseRepo.GetById(courseId);
            if(course == null)
            {
                ModelState.AddModelError("CrsID", "Course not found");
                return View();
            }

            var instructor = course.Ins.FirstOrDefault(i => i.InsID == instructorId);
            if (instructor == null)
            {
                ModelState.AddModelError("InsID", "Instructor not found");
                return View();
            }

            course.Ins.Remove(instructor);
            _unit.CourseRepo.Update(course);
            _unit.Save();

            return RedirectToAction("Details", new { id = courseId });
        }

        public IActionResult AddTopic(int CourseId, string TopicName)
        {
            if (string.IsNullOrEmpty(TopicName))
            {
                ModelState.AddModelError("TopicName", "Please enter a topic name.");
                return RedirectToAction("Details", new { id = CourseId });
            }

            var course = _unit.CourseRepo.GetById(CourseId);
            if (course == null)
            {
                ModelState.AddModelError("CrsID", "Course not found");
                return RedirectToAction("Details", new { id = CourseId });
            }

            var CourseTopic = new Courses_Topic
            {
                TopicName = TopicName,
                CrsID = CourseId
            };

            _unit.CourseRepo.AddTopic(CourseTopic);
            _unit.Save();

            return RedirectToAction("Details", new { id = CourseId });
        }

        public IActionResult DeleteTopic(int CourseId, string TopicName)
        {
            var course = _unit.CourseRepo.GetById(CourseId);
            if (course == null)
            {
                ModelState.AddModelError("CrsID", "Course not found");
                return RedirectToAction("Details", new { id = CourseId });
            }
            var topic = course.Courses_Topics.FirstOrDefault(t => t.TopicName == TopicName);
            if (topic == null)
            {
                ModelState.AddModelError("TopicName", "Topic not found");
                return RedirectToAction("Details", new { id = CourseId });
            }
            course.Courses_Topics.Remove(topic);
            _unit.CourseRepo.Update(course);
            _unit.Save();
            return RedirectToAction("Details", new { id = CourseId });
        }

        [HttpPost]
        public IActionResult EditTopic(int CourseId, string OldTopicName, string NewTopicName)
        {
            if(string.IsNullOrEmpty(NewTopicName))
            {
                ModelState.AddModelError("NewTopicName", "Please enter a new topic name.");
                return RedirectToAction("Details", new { id = CourseId });
            }

            var course = _unit.CourseRepo.GetById(CourseId);
            if (course == null)
            {
                ModelState.AddModelError("CrsID", "Course not found");
                return RedirectToAction("Details", new { id = CourseId });
            }

            var topic = course.Courses_Topics.FirstOrDefault(t => t.TopicName == OldTopicName);
            if (topic == null)
            {
                ModelState.AddModelError("TopicName", "Topic not found");
                return RedirectToAction("Details", new { id = CourseId });
            }

            var existingTopic = course.Courses_Topics.FirstOrDefault(t => t.TopicName == NewTopicName);
            if (existingTopic != null)
            {
                ModelState.AddModelError("NewTopicName", "This topic name already exists in the course.");
                return RedirectToAction("Details", new { id = CourseId });
            }

            course.Courses_Topics.Remove(topic);
            _unit.CourseRepo.Update(course);
            _unit.Save();


            var newTopic = new Courses_Topic
            {
                CrsID = CourseId,
                TopicName = NewTopicName
            };
            _unit.CourseRepo.AddTopic(newTopic);
            _unit.Save();

            return RedirectToAction("Details", new { id = CourseId });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _unit.CourseRepo.Add(course);
                _unit.Save();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Please fill all required fields.");
            return View(course);
        }

        public IActionResult Edit(int id)
        {
            var course = _unit.CourseRepo.GetById(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _unit.CourseRepo.Update(course);
                _unit.Save();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Please fill all required fields.");
            return View(course);
        }
    }
}
