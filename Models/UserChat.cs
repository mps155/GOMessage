using MongoDB.Bson;

namespace GOMessage.Models
{
    public class UserChat
    {
        public ObjectId Id { get; set; } // O Mongo sempre exige um _id para cada documento

        // Usamos string aqui para facilitar o tráfego dos IDs nas requisições, 
        // mas eles representarão os ObjectIds das outras coleções.
        public string UserId { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;

        // Dados extras valiosos para o seu portfólio:
        public string Role { get; set; } = "Member"; // Pode ser "Admin" ou "Member"
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
