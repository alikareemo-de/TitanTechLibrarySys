using Moq;
using TT.LibrarySys.DataAccess.Models;
using TT.LibrarySystem.BusinessLogic;
using Xunit;
namespace TestProject
{
    public class BookServiceTests
    {
        [Fact]
        public async Task GetAvailableBooksAsync_ShouldReturnOnlyAvailableBooks()
        {
            var mockBooksRepository = new Mock<IBooksRepository>();
            var mockBorrowingRepository = new Mock<IBorrowingRepository>();

            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book 1", IsAvailable = true },
            new Book { BookId = 2, Title = "Book 2", IsAvailable = false },
            new Book { BookId = 3, Title = "Book 3", IsAvailable = true }
        };

            mockBooksRepository.Setup(repo => repo.GetAllBooksAsync())
                               .ReturnsAsync(books);

            var bookService = new BooksService(mockBooksRepository.Object, mockBorrowingRepository.Object);

            var availableBooks = await bookService.GetAvailableBooksAsync();

            Xunit.Assert.Equal(2, availableBooks.Count);
            Xunit.Assert.All(availableBooks, b => Xunit.Assert.True(b.IsAvailable));
        }

        [Fact]
        public async Task GetAvailableBooksAsync_ShouldThrowException_WhenRepositoryFails()
        {
            var mockBooksRepository = new Mock<IBooksRepository>();
            var mockBorrowingRepository = new Mock<IBorrowingRepository>();

            mockBooksRepository.Setup(repo => repo.GetAllBooksAsync())
                               .ThrowsAsync(new Exception("Repository failure"));

            var bookService = new BooksService(mockBooksRepository.Object, mockBorrowingRepository.Object);

            await Xunit.Assert.ThrowsAsync<Exception>(async () => await bookService.GetAvailableBooksAsync());
        }

        [Fact]
        public async Task SearchBooksAsync_ShouldReturnMatchingBooks()
        {
            var mockBooksRepository = new Mock<IBooksRepository>();
            var mockBorrowingRepository = new Mock<IBorrowingRepository>();

            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "C# Programming", Author = "John Doe", ISBN = "12345" },
            new Book { BookId = 2, Title = "ASP.NET Core", Author = "Jane Doe", ISBN = "67890" },
            new Book { BookId = 3, Title = "C# Advanced", Author = "John Smith", ISBN = "54321" }
        };

            mockBooksRepository.Setup(repo => repo.SearchBooksAsync("C#"))
                               .ReturnsAsync(books.Where(b => b.Title.Contains("C#")).ToList());

            var bookService = new BooksService(mockBooksRepository.Object, mockBorrowingRepository.Object);

            var searchResults = await bookService.SearchBooksAsync("C#");

            Xunit.Assert.Equal(2, searchResults.Count);
            Xunit.Assert.All(searchResults, b => Xunit.Assert.Contains("C#", b.Title));
        }

        [Fact]
        public async Task SearchBooksAsync_ShouldThrowException_WhenRepositoryFails()
        {
            var mockBooksRepository = new Mock<IBooksRepository>();
            var mockBorrowingRepository = new Mock<IBorrowingRepository>();

            mockBooksRepository.Setup(repo => repo.SearchBooksAsync(It.IsAny<string>()))
                               .ThrowsAsync(new Exception("Repository failure"));

            var bookService = new BooksService(mockBooksRepository.Object, mockBorrowingRepository.Object);

            await Xunit.Assert.ThrowsAsync<Exception>(async () => await bookService.SearchBooksAsync("C#"));
        }
    }
}