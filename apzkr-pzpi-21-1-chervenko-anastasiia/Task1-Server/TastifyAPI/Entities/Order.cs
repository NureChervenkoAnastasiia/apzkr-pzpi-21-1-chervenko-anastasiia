using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("number"), BsonRepresentation(BsonType.Int32)]
        public int? number { get; set; }

        [BsonElement("table_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? TableId { get; set; }

        [BsonElement("date_time"), BsonRepresentation(BsonType.DateTime)]
        public DateTime OrderDateTime { get; set; }

        [BsonElement("comment"), BsonRepresentation(BsonType.String)]
        public string? Comment { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }

    }
}
