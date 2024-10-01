using Microsoft.AspNetCore.Mvc;
using TT.LibrarySys.DataAccess.Models;
using TT.LibrarySystem.BusinessLogic;
using TT.LibrarySystem.BusinessLogic.Services;

namespace TT.LibrarySys.Web.Controllers
{
    public static class LibraryControllerHelper
    {
        public static bool IsUserLoggedIn(Controller controller)
        {
            var userId = controller.HttpContext.Session.GetInt32("UserId");
            return userId != null;
        }

        public static void HandleError(Controller controller, Exception ex, string customErrorMessage)
        {
            controller.ModelState.AddModelError("", customErrorMessage);

        }

        public static async Task<bool> AreBooksAvailable(Controller controller, IBooksService _booksService, int[] bookIds)
        {
            var isBooksAvailable = await _booksService.IsBooksAvailableAsync(bookIds);
            if (!isBooksAvailable)
            {
                controller.ModelState.AddModelError("", "Some of the selected books are not available.");
                return false;
            }
            return true;
        }

        public static async Task<IActionResult> HandleRequestAction(
            int borrowingId,
            BorrowingStatus status,
            IBorrowingService Service,
            Controller controller,
            string successRedirectAction = "PendingRequests")
        {
            try
            {
                await Service.UpdateRequestStatusAsync(borrowingId, status);

                return controller.RedirectToAction(successRedirectAction);
            }
            catch (Exception ex)
            {
                controller.ModelState.AddModelError("", "An error occurred while processing the request.");

                return controller.RedirectToAction(successRedirectAction);
            }
        }



        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;

            return true;
        }

        public static void SetUserSession(Controller controller, User user)
        {
            controller.HttpContext.Session.SetInt32("UserId", user.UserId);
            controller.HttpContext.Session.SetString("UserName", user.UserName);
        }

        public static void ClearUserSession(Controller controller)
        {
            controller.HttpContext.Session.Clear();
        }
    }
}
