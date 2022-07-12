using System.Runtime.CompilerServices;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureSubscriptionService
{
	IAsyncEnumerable<ISubscription> ListSubscriptions([EnumeratorCancellation] CancellationToken cancellationToken);
}