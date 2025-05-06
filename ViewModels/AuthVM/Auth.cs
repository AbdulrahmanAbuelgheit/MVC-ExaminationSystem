using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.DTOs.AuthDTO
{
    public class Auth
    {
        public class RegisterUserVM
        {
            [Required]
            [StringLength(50)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]

            public string FirstName { get; set; }

            [Required]
            [StringLength(50)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]

            public string LastName { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Format")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(200, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$",
                ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase, 1 number, and 1 special character.")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime Birthdate { get; set; }

            [Required]
            [StringLength(50)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]

            public string Street { get; set; }

            [Required]
            [StringLength(50)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]

            public string Governate { get; set; }

            [Required]
            [StringLength(50)]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters.")]

            public string City { get; set; }

            [Required(ErrorMessage = "Branch is required.")]
            public int BranchID { get; set; }

            [Required(ErrorMessage = "Track is required.")]
            public int TrackID { get; set; }
            [Required(ErrorMessage ="Intake year is required.")]
            [Display(Name = "Intake Year")]
            public int IntakeYear { get; set; }

        }

        public class LoginUserVM
        {
            [Required, EmailAddress]
            public string Email { get; set; }
            [Required, DataType(DataType.Password)]
            public string Password { get; set; }
        }


    }
}
