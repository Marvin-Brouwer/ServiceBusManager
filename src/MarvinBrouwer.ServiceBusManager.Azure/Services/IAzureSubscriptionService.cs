using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// Service dedicated to Querying the account's Azure subscription
/// </summary>
public interface IAzureSubscriptionService
{
	/// <summary>
	/// List all the subscription the current authenticated user has access to.
	/// </summary>
	IAsyncEnumerable<IAzureSubscription> ListSubscriptions(CancellationToken cancellationToken);
}