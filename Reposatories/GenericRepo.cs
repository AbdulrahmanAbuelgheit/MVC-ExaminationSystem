using ExaminationSystemMVC.Models;

namespace ExaminationSystemMVC.Reposatories
{
    public class GenericRepo<TEntity> where TEntity : class
    {
        public DBContext Db { get; }
        public GenericRepo(DBContext db)
        {
            Db = db;
        }

        public List<TEntity> GetAll()
        {
            return Db.Set<TEntity>().ToList();
        }

        public TEntity GetById(int id)
        {
            return Db.Set<TEntity>().Find(id);
        }
        public void Add(TEntity entity)
        {
            Db.Set<TEntity>().Add(entity);
        }
        public void Delete(int id)
        {
            TEntity entity = GetById(id);
            if (entity != null)
            {
                Db.Set<TEntity>().Remove(entity);
            }
        }
        public void Update(TEntity entity)
        {
            Db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        }

    }
}
