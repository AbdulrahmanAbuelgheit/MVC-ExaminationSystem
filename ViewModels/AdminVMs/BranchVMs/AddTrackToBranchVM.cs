using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.BranchVMs
{
    public class AddTrackToBranchVM
    {
        public int BranchID { get; set; }
        public int TrackID { get; set; }

        public SelectList AvailableTracks { get; set; } 

    }
}
