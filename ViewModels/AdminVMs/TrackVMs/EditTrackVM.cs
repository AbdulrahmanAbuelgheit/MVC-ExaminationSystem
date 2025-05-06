using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.TrackVMs
{
    public class EditTrackVM
    {
        public int TrackID { get; set; }

        [Required(ErrorMessage = "Track name is required.")]
        [StringLength(50, ErrorMessage = "Track name cannot exceed 50 characters.")]
        public string TrackName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
        public int Duration { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }
    }
}
