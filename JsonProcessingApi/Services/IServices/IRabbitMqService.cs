namespace JsonProcessingApi.Services.IServices
{
    public interface IRabbitMqService
    {
        void SendMessage(string message);
        void Dispose();
    }
}
