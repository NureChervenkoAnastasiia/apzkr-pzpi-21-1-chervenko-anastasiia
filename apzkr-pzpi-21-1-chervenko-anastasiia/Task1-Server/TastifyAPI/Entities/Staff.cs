using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Staff
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("position"), BsonRepresentation(BsonType.String)]
        public string? Position { get; set; }

        [BsonElement("hourly_salary"), BsonRepresentation(BsonType.Double)]
        public double? HourlySalary { get; set; }

        [BsonElement("mobile_number"), BsonRepresentation(BsonType.String)]
        public string? Phone { get; set; }

        [BsonElement("attendance_card"), BsonRepresentation(BsonType.Int32)]
        public Int32? AttendanceCard { get; set; }

        [BsonElement("login"), BsonRepresentation(BsonType.String)]
        public string? Login { get; set; }

        [BsonElement("password_hash"), BsonRepresentation(BsonType.String)]
        public string? Password { get; set; }

        [BsonElement("restaurant_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? RestaurantId { get; set; }

    }
}
