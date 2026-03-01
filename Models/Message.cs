using MongoDB.Bson;

namespace GOMessage.Models
{
    public class Message
    {
        public ObjectId Id { get; set; }
        public string ChatId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
