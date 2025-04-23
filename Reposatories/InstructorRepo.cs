


namespace ExaminationSystemMVC.Reposatories
{
    public class InstructorRepo : GenericRepo<Instructor>
    {
        public InstructorRepo(DBContext Db) : base(Db)
        {
        }

        public List<Instructor> GetAllInstructors()
        {
            return Db.Instructors.Include(i => i.Ins).ToList();
        }


    }
}
