namespace ExaminationSystemMVC.Reposatories
{
    public class TrackRepo : GenericRepo<Track>
    {
        public TrackRepo(DBContext db) : base(db)
        {
        }

        public Track GetTrackWithDetails(int id)
        {
            return Db.Tracks.Include(t => t.Ins)
                .Include(t => t.Crs)
                .Include(t => t.Branches)
                .Include(t => t.Students)
                .ThenInclude(s => s.Std)
                .FirstOrDefault(t => t.TrackID == id);
        }


        public Track GetTrackWithCourses(int id)
        {
            return Db.Tracks.Include(t => t.Crs).FirstOrDefault(t => t.TrackID == id);
        }

        public Track GetTrackWithCoursesAndStudents(int id)
        {
            return Db.Tracks
                .Include(t => t.Crs)
                .Include(t => t.Students)
                .FirstOrDefault(t => t.TrackID == id);
        }

        public Track GetTrackWithStudents(int id)
        {
            return Db.Tracks.Include(t => t.Students)
                .FirstOrDefault(t => t.TrackID == id);
        }

        public Track GetTrackWithInstructors(int id)
        {
            return Db.Tracks.Include(t => t.Ins).FirstOrDefault(t => t.TrackID == id);
        }

        public Track GetByIdWithBranches(int id)
        {
            return Db.Tracks
                .Include(t => t.Branches)
                .FirstOrDefault(t => t.TrackID == id);
        }
        
        public bool SafeToDelete(int trackId)
        {
            var track = Db.Tracks.Include(t => t.Ins)
                .Include(t => t.Branches)
                .FirstOrDefault(t => t.TrackID == trackId);

            if (track == null)
                return false;
            // No instructors AND no branches
            if (track.Ins.Count == 0 && track.Branches.Count == 0)
                return true;

            // Only supervisor as instructor AND no branches
            if (track.Ins.Count == 1 && track.SupervisorID == track.Ins.First().InsID && track.Branches.Count == 0)
                return true;

            // Otherwise, not safe
            return false;
        }

    }
}
