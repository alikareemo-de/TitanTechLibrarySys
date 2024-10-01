using Microsoft.AspNetCore.Mvc;
using TT.LibrarySys.DataAccess.Models;
using TT.LibrarySystem.BusinessLogic;
using TT.LibrarySystem.BusinessLogic.Services;

namespace TT.LibrarySys.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly IBorrowingService _borrowingService;

        public AdminController(IBorrowingService borrowingService, IBooksService booksService)
        {
            _borrowingService = borrowingService;
            _booksService = booksService;
        }

        public async Task<IActionResult> PendingRequests()
        {
            try
            {
                var pendingRequests = await _borrowingService.GetPendingBorrowingsAsync();
                return View(pendingRequests);
            }
            catch (Exception ex)
            {
                LibraryControllerHelper.HandleError(this, ex, "An error occurred while fetching pending requests.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int borrowingId, BorrowingStatus status)
        {
            return await LibraryControllerHelper.HandleRequestAction(borrowingId, status, _borrowingService, this);
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int borrowingId, BorrowingStatus status)
        {
            return await LibraryControllerHelper.HandleRequestAction(borrowingId, status, _borrowingService, this);
        }
    }
}
