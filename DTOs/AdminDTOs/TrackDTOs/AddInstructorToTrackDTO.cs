using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.TrackDTOs
{
    public class AddInstructorToTrackDTO
    {
        public int TrackID { get; set; }
        [Required(ErrorMessage = "Please select an instructor.")]
        public int InsID { get; set; }
       
    }
}
