using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Guest
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("mobile_number"), BsonRepresentation(BsonType.String)]
        public string? Phone { get; set; }

        [BsonElement("bonus"), BsonRepresentation(BsonType.Int32)]
        public Int32 Bonus { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public string? Email { get; set; }

        [BsonElement("password_hash"), BsonRepresentation(BsonType.String)]
        public string? Password { get; set; }

        [BsonElement("login"), BsonRepresentation(BsonType.String)]
        public string? Login {  get; set; } 
    }
}
