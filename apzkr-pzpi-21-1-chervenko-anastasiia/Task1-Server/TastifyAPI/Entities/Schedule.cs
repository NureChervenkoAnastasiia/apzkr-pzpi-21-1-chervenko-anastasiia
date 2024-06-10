using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Schedule
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("staff_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? StaffId { get; set; }

        [BsonElement("start_date_time"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? StartDateTime { get; set; }

        [BsonElement("finish_date_time"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? FinishDateTime { get; set; }
    }
}
