namespace ExaminationSystemMVC.Reposatories
{
    public class CourseRepo : GenericRepo<Course>
    {
        public CourseRepo(DBContext context) : base(context)
        {
        }

        public List<Course> GetAllWithTopics()
        {
            return Db.Courses.Include(c => c.Courses_Topics).ToList();
        }

        public Course GetByIdWithTopicsAndInstructors(int id)
        {
            return Db.Courses
                .Include(c => c.Courses_Topics)
                .Include(c => c.Ins)
                .FirstOrDefault(c => c.CrsID == id);
        }

        public void AddTopic(Courses_Topic topic)
        {
            Db.Courses_Topics.Add(topic);
        }
    }
}
