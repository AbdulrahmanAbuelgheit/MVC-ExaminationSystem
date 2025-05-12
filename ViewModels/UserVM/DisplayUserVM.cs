namespace ExaminationSystemMVC.DTOs.UserDTO
{
    public class DisplayUserVM
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime Birthdate { get; set; }
        public string Street { get; set; } = null!;
        public string Governate { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; }
    }
}
