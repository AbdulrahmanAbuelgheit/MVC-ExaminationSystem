using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.DTOs.AdminDTOs.TrackDTOs
{
    public class AddCourseToTrackDTO
    {
        public int TrackID { get; set; }
        public int CourseID { get; set; }
    }
}
