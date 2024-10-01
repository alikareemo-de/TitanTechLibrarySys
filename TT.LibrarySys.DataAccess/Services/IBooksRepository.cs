
namespace TT.LibrarySys.DataAccess.Models
{
    public interface IBooksRepository
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<List<Book>> GetAvailableBooksAsync();
        Task<List<Book>> SearchBooksAsync(string searchTerm);

        Task UpdateBookAsync(Book book);

        Task<Book> GetBookByIdAsync(int bookId);
        Task UpdateBookAvailabilityAsync(int bookId, bool isAvailable);
    }
}
