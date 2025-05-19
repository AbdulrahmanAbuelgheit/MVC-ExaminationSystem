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
        public IEnumerable<Student> GetByBranchId(int branchId)
        {
            return Db.Students
                .Include(s => s.Std)
                .Where(s => s.BranchID == branchId)
                .ToList();
        }
        // In StudentRepository.cs
        public IEnumerable<Student> GetByTrackAndBranch(int trackId, int branchId)
        {
            return Db.Students
                .Include(s => s.Std)
                .Where(s => s.TrackID == trackId && s.BranchID == branchId)
                .ToList();
        }

        public IEnumerable<Student> GetByBranch(int branchId)
        {
            return Db.Students
                .Include(s => s.Std)
                .Where(s => s.BranchID == branchId)
                .ToList();
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

        public List<Exam> getStudentExam(int id)
        {
            // stdid

            Student StdWithCourse = Db.Students.Include(s => s.Student_Courses).FirstOrDefault(s => s.StdID == id);

            var exams = Db.Exams.Where( s => s.CrsID == StdWithCourse.Student_Courses.FirstOrDefault().CrsID).ToList();

            return exams;
            //return Db.Exams.Where(s  == id).Include(se => se.Exam).ThenInclude(e => e.Crs).ToList();
        }

    }
}
