using System.ComponentModel.DataAnnotations;

namespace ExaminationSystemMVC.DTOs.AuthDTO
{
    public class Auth
    {
        public class RegisterUserVM
        {
            [Required]
            [StringLength(50)]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50)]
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
            public string Street { get; set; }

            [Required]
            [StringLength(50)]
            public string Governate { get; set; }

            [Required]
            [StringLength(50)]
            public string City { get; set; }
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
