using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.BranchDTOs
{
    public class DisplayBranchDTO
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }

        public string Governate { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        [Required]
        public int Tel { get; set; }

        public int ManagerID { get; set; }
        public string ManagerName { get; set; }

        public List<Track> Tracks { get; set; } = new List<Track>();
    }
}
