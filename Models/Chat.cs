using MongoDB.Bson;

namespace GOMessage.Models
{
    public class Chat
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsGroupChat { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
