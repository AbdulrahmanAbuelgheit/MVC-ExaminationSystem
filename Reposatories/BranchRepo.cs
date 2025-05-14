namespace ExaminationSystemMVC.Reposatories
{
    public class BranchRepo : GenericRepo<Branch>
    {
        public BranchRepo(DBContext Db) : base(Db)
        {
        }

        public Branch GetBranchByIdWithTracks(int id)
        {
            return Db.Branches.Include(b => b.Tracks)
                .Include(b => b.Ins)
                .Include(b => b.Manager)
                .ThenInclude(m => m.Ins)
                .FirstOrDefault(b => b.BranchID == id);
        }

        public List<Instructor> GetInstructorsInBranch(int id)
        {
            return Db.Instructors.Include(i => i.Ins).Where(i => i.Branches.Any(i => i.BranchID == id)).ToList();
        }

        public Branch GetBranchWithInstructors(int id)
        {
            return Db.Branches.Include(b => b.Ins).First(i => i.BranchID == id);

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
