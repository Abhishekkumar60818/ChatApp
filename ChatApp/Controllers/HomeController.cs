using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChatAppDbContext _context;

        public HomeController(ChatAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery)
        {
            // Fetch the filtered list based on the search query
            var list = await _context.Users
                .Where(x => string.IsNullOrEmpty(searchQuery) ||
                            x.Name.Contains(searchQuery))
                .Select(x => new User
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    ProfileImagePath = x.ProfileImagePath,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            ViewBag.SearchQuery = searchQuery; // Pass the search query to the view for UI display

            return View(list);
        }
    }
}
