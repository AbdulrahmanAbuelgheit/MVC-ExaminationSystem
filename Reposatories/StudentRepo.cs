using ExaminationSystemMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Reposatories
{
    public class StudentRepo : GenericRepo<Student>
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

    }
}
