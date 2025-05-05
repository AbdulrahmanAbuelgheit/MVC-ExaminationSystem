using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.ViewModels.AdminVMs.BranchVMs
{
    public class CreateBranchVM
    {
        public int? BranchID { get; set; }
        [Required, StringLength(50)]
        public string BranchName { get; set; }

        [Required, StringLength(50)]
        public string Governate { get; set; }

        [Required, StringLength(50)]
        public string City { get; set; }

        [Required, StringLength(50)]
        public string Street { get; set; }

        public int Tel { get; set; }

        [Required]
        public int ManagerID { get; set; }
    }

}
