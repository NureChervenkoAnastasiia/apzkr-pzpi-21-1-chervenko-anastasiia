using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Booking
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("table_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? TableId { get; set; }

        [BsonElement("guest_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? GuestId { get; set; }

        [BsonElement("date_time"), BsonRepresentation(BsonType.DateTime)]
        public DateTime BookingDateTime { get; set; }

        [BsonElement("persons"), BsonRepresentation(BsonType.Int32)]
        public Int32? PersonsCount { get; set; }

        [BsonElement("comment"), BsonRepresentation(BsonType.String)]
        public string? Comment { get; set; }

    }
}
