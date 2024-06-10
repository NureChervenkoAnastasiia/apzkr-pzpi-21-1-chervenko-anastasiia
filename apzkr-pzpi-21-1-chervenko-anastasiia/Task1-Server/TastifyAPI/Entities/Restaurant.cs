using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Restaurant
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("address"), BsonRepresentation(BsonType.String)]
        public string? Address { get; set; }

        [BsonElement("telephone"), BsonRepresentation(BsonType.String)]
        public string? Phone { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public string? Email { get; set; }

        [BsonElement("info"), BsonRepresentation(BsonType.String)]
        public string? Info { get; set; }

        [BsonElement("cuisine")]
        public List<string>? Cuisine { get; set; }
    }
}
