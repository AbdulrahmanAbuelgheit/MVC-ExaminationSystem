using ExaminationSystemMVC.Models;

namespace ExaminationSystemMVC.Reposatories
{
    public class UsersRepo:GenericRepo<User>
    {
        public UsersRepo(DBContext db) : base(db)
        {
        }

        public User GetByEmail(string email)
        {
            return Db.Users.FirstOrDefault(u => u.Email == email);
        }


    }
}
