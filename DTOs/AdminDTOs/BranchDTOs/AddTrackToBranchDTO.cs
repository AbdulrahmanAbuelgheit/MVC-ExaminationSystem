using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.BranchDTOs
{
    public class AddTrackToBranchDTO
    {
        public int BranchID { get; set; }
        public int TrackID { get; set; }

        public SelectList AvailableTracks { get; set; } 

    }
}
