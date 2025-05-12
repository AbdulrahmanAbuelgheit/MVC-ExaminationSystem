namespace ExaminationSystemMVC.Reposatories
{
    public class CourseRepo: GenericRepo<Course>
    {
        public CourseRepo(DBContext Db) : base(Db)
    {
    }
    
        public Course GetCourseWithInstructors(int id)
        {
            return Db.Courses
                .Include(c => c.Ins)
                .ThenInclude(i => i.Ins) // Include user details if needed
                .FirstOrDefault(c => c.CrsID == id);
        }
    }
}
