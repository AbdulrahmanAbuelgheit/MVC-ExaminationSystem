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
                .ThenInclude(t => t.Ins)
                .Include(b => b.Ins)
                .Include(b => b.Manager)
                .FirstOrDefault(b => b.BranchID == id);
        }

        public Branch GetBranchWithInstructors(int id)
        {
            return Db.Branches.Include(b => b.Ins).First(i => i.BranchID == id);

        }

        public Branch GetBranchWithTrackAndInstructors(int id)
        {
            return Db.Branches.Include(b => b.Tracks)
                .Include(b => b.Ins)
                .FirstOrDefault(b => b.BranchID == id);
        }
        public int GetBranchIdByManagerId(int managerId)
        {
            return Db.Branches
                .Where(b => b.ManagerID == managerId)
                .Select(b => b.BranchID)
                .FirstOrDefault();
        }

        public bool SafeToDelete(int trackId, int branchId)
        {
            var branch = Db.Branches.Include(b => b.Tracks).Include(b => b.Ins).FirstOrDefault(b => b.BranchID == branchId);

            var track = Db.Tracks.Include(t => t.Ins).FirstOrDefault(t => t.TrackID == trackId);

            if (branch == null || track == null)
                return false;

            var instructorsInBoth = track.Ins
                .Where(ins => branch.Ins.Any(bi => bi.InsID == ins.InsID))
                .ToList();

            return instructorsInBoth.Count == 0;
        }

    }
}
