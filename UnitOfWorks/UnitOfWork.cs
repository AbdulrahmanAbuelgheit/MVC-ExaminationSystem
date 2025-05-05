
namespace ExaminationSystemMVC.UnitOfWorks
{
    public class UnitOfWork
    {
        GenericRepo<Course> _courseRepo;
        BranchRepo _branchRepo;
        TrackRepo _trackRepo;
        StudentRepo _studentRepo;
        InstructorRepo _instructorRepo;
        UsersRepo _userRepo;
        DBContext _db;
        public UnitOfWork(DBContext db)
        {
            _db = db;
        }
        public UsersRepo UserRepo
        {
            get
            {
                if (_userRepo == null)
                {
                    _userRepo = new UsersRepo(_db);
                }
                return _userRepo;
            }
        }

        public GenericRepo<Course> CourseRepo
        {
            get
            {
                if (_courseRepo == null)
                {
                    _courseRepo = new GenericRepo<Course>(_db);
                }
                return _courseRepo;
            }
        }

        public BranchRepo BranchRepo
        {
            get
            {
                if (_branchRepo == null)
                {
                    _branchRepo = new BranchRepo(_db);
                }
                return _branchRepo;
            }
        }

        public TrackRepo TrackRepo
        {
            get
            {
                if (_trackRepo == null)
                {
                    _trackRepo = new TrackRepo(_db);
                }
                return _trackRepo;
            }
        }

        public InstructorRepo InstructorRepo
        {
            get
            {
                if (_instructorRepo == null)
                {
                    _instructorRepo = new InstructorRepo(_db);
                }
                return _instructorRepo;
            }
        }

        public StudentRepo StudentRepo
        {
            get
            {
                if (_studentRepo == null)
                {
                    _studentRepo = new StudentRepo(_db);
                }
                return _studentRepo;
            }
        }
        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
