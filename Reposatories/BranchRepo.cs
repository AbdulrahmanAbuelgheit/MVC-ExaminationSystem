namespace ExaminationSystemMVC.Reposatories
{
    public class BranchRepo : GenericRepo<Branch>
    {
        public BranchRepo(DBContext Db) : base(Db)
        {
        }

        public Branch GetBranchByIdWithTracks(int id)
        {
            return Db.Branches.Include(b => b.Tracks).FirstOrDefault(b => b.BranchID == id);
        }
        public int GetBranchIdByManagerId(int managerId)
        {
            return Db.Branches
                .Where(b => b.ManagerID == managerId)
                .Select(b => b.BranchID)
                .FirstOrDefault();
        }

    }
}
