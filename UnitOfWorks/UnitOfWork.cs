using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.Reposatories;

namespace ExaminationSystemMVC.UnitOfWorks
{
    public class UnitOfWork
    {
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
        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
