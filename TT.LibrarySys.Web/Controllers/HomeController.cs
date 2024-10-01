using Microsoft.AspNetCore.Mvc;
using TT.LibrarySys.DataAccess.Models;

namespace TT.LibrarySys.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBooksRepository _booksRepository;

        public HomeController(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _booksRepository.GetAllBooksAsync();
            return View(books);
        }
    }
}
