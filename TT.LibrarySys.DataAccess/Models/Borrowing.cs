
namespace TT.LibrarySys.DataAccess.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }

        public BorrowingStatus Status { get; set; }

        public User? User { get; set; }
        public Book? Book { get; set; }
    }
}
