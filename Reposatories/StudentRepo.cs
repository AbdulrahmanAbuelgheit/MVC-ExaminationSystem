using ExaminationSystemMVC.Models;
using Microsoft.EntityFrameworkCore;


namespace ExaminationSystemMVC.Reposatories
{
    public  class StudentRepo : GenericRepo<Student>
    {
        public StudentRepo(DBContext Db) : base(Db)
        {
        }

        public List<Student> GetAllWithBranchAndTrack()
        {
            return Db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).ToList();
        }

        public Student GetByIdWithBranchAndTrack(int id)
        {
            return Db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).FirstOrDefault(s => s.StdID == id);
        }

        public void SoftDelete(int id)
        {
            var student = GetById(id);
            if (student != null)
            {
                student.IsActive = "F";
                Update(student);
            }
        }

        public Student getById(int id)
        {
            return Db.Students.Include(s => s.Std).Include(s => s.Branch).Include(s => s.Track).FirstOrDefault(s => s.StdID == id);
        }

        public Student getByEmail(string email)
        {
            return Db.Students.Include(s => s.Std).FirstOrDefault(s => s.Std.Email == email);
        }
        public List<Student_Course> getStudentCourse(int id)
        {
            return Db.Student_Courses
                .Where(sc => sc.StdID == id)
                .Include(sc => sc.Crs)
                .ToList();
        }

        public List<Student_Exam> getStudentExam(int id)
        {
            return Db.Student_Exams.Where(se => se.StdID == id).Include(se => se.Exam).ThenInclude(e => e.Crs).ToList();
        }

    }
}
