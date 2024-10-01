using Microsoft.AspNetCore.Mvc;
using TT.LibrarySys.DataAccess.Models;

namespace TT.LibrarySys.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user, string password)
        {
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Borrowings");

            if (ModelState.IsValid)
            {
                if (!LibraryControllerHelper.IsPasswordValid(password))
                {
                    ModelState.AddModelError("Password", "Password must be at least 8 characters long, contain an uppercase letter, a lowercase letter, and a digit.");
                    return View(user);
                }

                try
                {
                    await _userRepository.SignUpAsync(user, password);
                    return RedirectToAction("SignIn");
                }
                catch (Exception ex)
                {
                    LibraryControllerHelper.HandleError(this, ex, "An error occurred while trying to sign up. Please try again later.");
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userRepository.SignInAsync(username, password);

                    if (user != null)
                    {
                        LibraryControllerHelper.SetUserSession(this, user);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    LibraryControllerHelper.HandleError(this, ex, "An error occurred while trying to sign in. Please try again later.");
                }
            }

            return View();
        }

        public IActionResult SignOut()
        {
            LibraryControllerHelper.ClearUserSession(this);
            return RedirectToAction("SignIn");
        }
    }
}
