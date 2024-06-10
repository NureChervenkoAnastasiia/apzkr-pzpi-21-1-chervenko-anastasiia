using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class PositionProduct
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("menu_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? MenuId { get; set; }

        [BsonElement("product_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }

    }
}
