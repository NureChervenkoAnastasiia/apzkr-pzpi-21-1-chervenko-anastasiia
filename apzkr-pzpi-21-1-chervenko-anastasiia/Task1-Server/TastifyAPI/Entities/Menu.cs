using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Menu
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("restaurant_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? RestaurantId { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("size"), BsonRepresentation(BsonType.Int32)]
        public Int32 Size { get; set; }

        [BsonElement("price"), BsonRepresentation(BsonType.Int32)]
        public Int32? Price { get; set; }

        [BsonElement("info"), BsonRepresentation(BsonType.String)]
        public string? Info { get; set; }

        [BsonElement("type"), BsonRepresentation(BsonType.String)]
        public string? Type { get; set; }

    }
}
