using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            // Send the message to the specific receiver
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, message);
        }

        public override async Task OnConnectedAsync()
        {
            // Map the connection ID to the user ID
            string userId = Context.GetHttpContext().Session.GetInt32("UserId")?.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }
    }
}
