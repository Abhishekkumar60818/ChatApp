using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ChatAppDbContext _context;

        public UserController(ChatAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, IFormFile profileImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("PhoneNumber", "Phone number already exists");
                        return View(user);
                    }

                    if (profileImage != null && profileImage.Length > 0)
                    {
                        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                        var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await profileImage.CopyToAsync(stream);
                        }
                        user.ProfileImagePath = $"/uploads/{fileName}";
                    }
                    else
                    {
                        user.ProfileImagePath = "/uploads/default.png";
                    }

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            return View(user);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string phoneNumber, int? id)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number is required");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber || u.Id == id);
            if (user == null)
            {
                ModelState.AddModelError("PhoneNumber", "User not found.");
                return View();
            }

            // Store user details in the session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserImage", user.ProfileImagePath); // Assuming `ImageUrl` is a property in the `Users` table
            HttpContext.Session.SetString("uName", user.Name);
            // Redirect to the chat page or home page
            return RedirectToAction("Index", "Chat");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Index", "Home");
        }

    }
}
