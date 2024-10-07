

using TT.LibrarySys.DataAccess.Models;

namespace TT.LibrarySystem.BusinessLogic
{
    public class BooksService : IBooksService
    {

        private readonly IBooksRepository _booksRepository;

        public BooksService(IBooksRepository booksRepository)
        {

            _booksRepository = booksRepository;
        }

        public async Task<List<Book>> GetAvailableBooksAsync()
        {
            var books = await _booksRepository.GetAllBooksAsync();

            var availableBooks = books.Where(b => b.IsAvailable).ToList();
            return availableBooks;
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm) => await _booksRepository.SearchBooksAsync(searchTerm);



        public async Task UpdateBookAsync(Book book)
        {
            await _booksRepository.UpdateBookAsync(book);
        }





        public async Task<bool> IsBooksAvailableAsync(int[] bookIds)
        {
            var books = await _booksRepository.GetAllBooksAsync();

            return bookIds.All(bookId => books.Any(b => b.BookId == bookId && b.IsAvailable));
        }



        public async Task UpdateBookAvailabilityAsync(int bookId, bool isAvailable)
        {
            await _booksRepository.UpdateBookAvailabilityAsync(bookId, isAvailable);
        }

    }
}
