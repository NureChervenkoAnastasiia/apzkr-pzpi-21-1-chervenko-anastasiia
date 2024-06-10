using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Table
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("number"), BsonRepresentation(BsonType.Int32)]
        public int? Number { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }

    }
}
