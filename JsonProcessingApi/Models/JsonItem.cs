using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JsonProcessingApi.Models
{
    public class JsonItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("U_TrackingNo")]
        public string U_TrackingNo { get; set; }

        [BsonElement("processedAt")]
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}