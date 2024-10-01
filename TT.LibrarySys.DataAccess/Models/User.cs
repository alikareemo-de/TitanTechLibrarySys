using System.ComponentModel.DataAnnotations;

namespace TT.LibrarySys.DataAccess.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        public string? Password { get; set; }


        public string? PasswordHash { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        public List<Borrowing>? Borrowings { get; set; }
    }
}
