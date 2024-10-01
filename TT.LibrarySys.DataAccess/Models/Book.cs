namespace TT.LibrarySys.DataAccess.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public bool IsAvailable { get; set; } = true;


        public List<Borrowing>? Borrowings { get; set; }
    }
}
