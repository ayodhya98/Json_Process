using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MessageConsumerApi.Services.IServices
{
    public interface IRabbitMqConsumerService : IHostedService
    {
        //        //Task StartAsync(CancellationToken cancellationToken);
        //        //Task StopAsync(CancellationToken cancellationToken);
    }
}