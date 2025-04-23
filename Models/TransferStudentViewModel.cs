using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExaminationSystemMVC.Models
{
    public class TransferStudentViewModel
    {
        public int StudentId { get; set; }
        public int CurrentTrackId { get; set; } // For dropdown

        [Display(Name = "New Track")]
        public int NewTrackId { get; set; }

        public string StudentName { get; set; }
        public SelectList AvailableTracks { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>()); // For dropdown
    }
}
