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

        public Track GetTrackWithStudents(int id)
        {
            return Db.Tracks.Include(t => t.Students)
                .FirstOrDefault(t => t.TrackID == id);
        }

        public Track GetTrackWithInstructors(int id)
        {
            return Db.Tracks.Include(t => t.Ins).FirstOrDefault(t => t.TrackID == id);
        }



    }
}
