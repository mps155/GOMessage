using MongoDB.Bson;

namespace GOMessage.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
