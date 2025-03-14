﻿using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatAppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(ChatAppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }


        public async Task<IActionResult> Index(string searchQuery)
        {
            var query = _context.Users.AsQueryable();
            var list = await query
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


        public async Task<IActionResult> GetChatMessages(int userId, int otherUserId)
        {
            // Ensure both user IDs exist in the database
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            var otherUserExists = await _context.Users.AnyAsync(u => u.Id == otherUserId);

            if (!userExists || !otherUserExists)
            {
                return NotFound("One or both users not found.");
            }

            var messages = await _context.Chats
                .Where(c => (c.SenderId == userId && c.ReceiverId == otherUserId) || (c.SenderId == otherUserId && c.ReceiverId == userId))
                .OrderBy(c => c.Timestamp)
                .Select(c => new
                {
                    c.Id,
                    c.Message,
                    IsSender = c.SenderId == userId,
                    c.Timestamp
                })
                .ToListAsync();

            return Ok(messages);
        }



        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Chat chat)
        {
            if (chat == null)
                return BadRequest("Invalid chat message.");

            var senderExists = await _context.Users.AnyAsync(u => u.Id == chat.SenderId);
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == chat.ReceiverId);

            if (!senderExists || !receiverExists)
            {
                return BadRequest("One or both users not found.");
            }

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            // Notify all clients about the new message
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", chat.SenderId, chat.Message);

            return Ok(new
            {
                chat.Id,
                chat.Message,
                IsSender = true,
                chat.Timestamp
            });
        }
    }

}

