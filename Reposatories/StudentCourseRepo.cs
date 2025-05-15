
namespace ExaminationSystemMVC.Reposatories
{
    public class StudentCourseRepo : GenericRepo<Student_Course>
    {
        public StudentCourseRepo(DBContext db) : base(db)
        {
        }
        public void AddRange(IEnumerable<Student_Course> entities)
        {
            Db.Set<Student_Course>().AddRange(entities);
        }

        public void RemoveRange(IEnumerable<Student_Course> entities)
        {
            Db.Set<Student_Course>().RemoveRange(entities);
        }


    }
}
