namespace ChatApp.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // User's full name
        public string PhoneNumber { get; set; } // Unique phone number
        public string? ProfileImagePath { get; set; } // Path to the profile image
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Account creation date
    }
}
