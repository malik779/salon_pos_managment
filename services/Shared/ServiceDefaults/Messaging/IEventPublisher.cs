using System.Threading;
using System.Threading.Tasks;

namespace Salon.ServiceDefaults.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T payload, CancellationToken cancellationToken = default);
}
