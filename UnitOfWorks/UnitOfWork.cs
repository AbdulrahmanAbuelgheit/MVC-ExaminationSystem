using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.Reposatories;

namespace ExaminationSystemMVC.UnitOfWorks
{
    public class UnitOfWork
    {
        GenericRepo<User> _userRepo;
        DBContext _db;
        public UnitOfWork(DBContext db)
        {
            _db = db;
        }
        public GenericRepo<User> UserRepo
        {
            get
            {
                if (_userRepo == null)
                {
                    _userRepo = new GenericRepo<User>(_db);
                }
                return _userRepo;
            }
        }
        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
