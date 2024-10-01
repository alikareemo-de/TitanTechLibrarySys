using Microsoft.AspNetCore.Mvc;
using TT.LibrarySystem.BusinessLogic;
using TT.LibrarySystem.BusinessLogic.Services;

namespace TT.LibrarySys.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly IBorrowingService _borrowingService;

        public BooksController(IBorrowingService borrowingService, IBooksService booksService)
        {
            _borrowingService = borrowingService;
            _booksService = booksService;
        }
        public async Task<IActionResult> AvailableBooks()
        {
            try
            {
                var availableBooks = await _booksService.GetAvailableBooksAsync();
                return View(availableBooks);
            }
            catch (Exception ex)
            {
                LibraryControllerHelper.HandleError(this, ex, "An error occurred while fetching available books.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBooks(int[] bookIds)
        {
            if (bookIds == null || bookIds.Length == 0)
            {
                ModelState.AddModelError("", "You must select at least one book.");
                return RedirectToAction("AvailableBooks");
            }

            try
            {
                if (!LibraryControllerHelper.IsUserLoggedIn(this))
                {
                    return RedirectToAction("SignIn", "Account");
                }

                if (!await LibraryControllerHelper.AreBooksAvailable(this, _booksService, bookIds))
                {
                    return RedirectToAction("AvailableBooks");
                }

                var userId = HttpContext.Session.GetInt32("UserId");

                // Proceed with adding the borrowings
                await _borrowingService.AddBorrowingsAsync(bookIds, userId.Value);

                return RedirectToAction("AvailableBooks");
            }
            catch (Exception ex)
            {
                LibraryControllerHelper.HandleError(this, ex, "An error occurred while processing your request.");
                return RedirectToAction("AvailableBooks");
            }
        }

        public async Task<IActionResult> SearchBooks(string searchString)
        {
            try
            {
                var books = await _booksService.SearchBooksAsync(searchString);
                return View("AvailableBooks", books);
            }
            catch (Exception ex)
            {
                LibraryControllerHelper.HandleError(this, ex, "An error occurred while searching for books.");
                return View("Error");
            }
        }
    }
}
