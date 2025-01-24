namespace ChatApp.Models
{
    public class Chat
    {
        public int Id { get; set; } // Primary Key
        public int SenderId { get; set; } // Foreign Key: User who sent the message
        public int ReceiverId { get; set; } // Foreign Key: User who received the message
        public string Message { get; set; } // Chat message content
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Time when the message was sent

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
