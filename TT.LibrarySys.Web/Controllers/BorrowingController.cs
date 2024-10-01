using Microsoft.AspNetCore.Mvc;
using TT.LibrarySystem.BusinessLogic;
using TT.LibrarySystem.BusinessLogic.Services;

namespace TT.LibrarySys.Web.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly IBorrowingService _borrowingService;

        public BorrowingController(IBorrowingService borrowingService, IBooksService booksService)
        {
            _borrowingService = borrowingService;
            _booksService = booksService;
        }

        public async Task<IActionResult> BorrowedBooks()
        {
            try
            {
                if (!LibraryControllerHelper.IsUserLoggedIn(this))
                {
                    return RedirectToAction("SignIn", "Account");
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                var borrowings = await _borrowingService.GetBorrowingsByUserIdAsync((int)userId);
                return View(borrowings);
            }
            catch (Exception ex)
            {
                LibraryControllerHelper.HandleError(this, ex, "An error occurred while fetching borrowed books. Please try again later.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(int borrowingId)
        {
            try
            {
                if (!LibraryControllerHelper.IsUserLoggedIn(this))
                {
                    return RedirectToAction("SignIn", "Account");
                }

                await _borrowingService.UpdateReturnDateAsync(borrowingId, DateTime.Now);

                return RedirectToAction("BorrowedBooks");
            }
            catch (Exception ex)
            {
                LibraryControllerHelper.HandleError(this, ex, "An error occurred while returning the book. Please try again later.");
                return RedirectToAction("BorrowedBooks");
            }
        }
    }
}
