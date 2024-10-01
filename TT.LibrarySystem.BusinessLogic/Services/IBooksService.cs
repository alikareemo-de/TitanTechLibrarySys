

using TT.LibrarySys.DataAccess.Models;

namespace TT.LibrarySystem.BusinessLogic
{
    public interface IBooksService
    {
        Task<List<Book>> SearchBooksAsync(string searchTerm);
        Task<bool> IsBooksAvailableAsync(int[] bookIds);
        Task<List<Book>> GetAvailableBooksAsync();
        Task UpdateBookAvailabilityAsync(int bookId, bool isAvailable);

    }
}
