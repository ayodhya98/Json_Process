namespace MessageConsumerApi.Models
{
    public class MessageModel
    {
        public string Content { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
