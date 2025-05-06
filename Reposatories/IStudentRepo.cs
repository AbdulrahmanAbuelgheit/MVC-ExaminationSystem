using ExaminationSystemMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Reposatories
{
    public interface IStudentRepo
    {
        Student getById(int id);
        Student getByEmail(string email);
        List<Student_Course> getStudentCourse(int id);
        List<Student_Exam> getStudentExam(int id);
    }

    public class StudentRepo : IStudentRepo
    {
        DBContext db;
        public StudentRepo(DBContext _db)
        {
            db = _db;
        }

        public Student getById(int id)
        {
            return db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).FirstOrDefault(s => s.StdID == id);
        }

        public Student getByEmail(string email)
        {
            return db.Students.Include(s => s.Std).FirstOrDefault(s => s.Std.Email == email);
        }
        public List<Student_Course> getStudentCourse(int id)
        {
            return db.Student_Courses
                .Where(sc => sc.StdID == id)
                .Include(sc => sc.Crs)
                .ToList();
        }

        public List<Student_Exam> getStudentExam(int id)
        {
            return db.Student_Exams.Where(se => se.StdID == id).Include(se => se.Exam).ThenInclude(e => e.Crs).ToList();
        }
    }
}
