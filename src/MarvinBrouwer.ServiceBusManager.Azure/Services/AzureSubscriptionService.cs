using Microsoft.Azure.Management.ResourceManager.Fluent.Models;

using System.Runtime.CompilerServices;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureSubscriptionService : IAzureSubscriptionService
{
	private readonly IAzureConnectionService _azureConnectionService;

	/// <inheritdoc cref="AzureSubscriptionService" />
	public AzureSubscriptionService(IAzureConnectionService azureConnectionService)
	{
		_azureConnectionService = azureConnectionService;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<IAzureSubscription> ListSubscriptions(AzureConnection connection, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var activeConnection = !connection.Expired
			? connection
			: await _azureConnectionService.RefreshConnection(connection, cancellationToken);

		var subscriptions = await activeConnection
			.AzureClient
			.Subscriptions
			.ListAsync(true, cancellationToken);

		foreach (var subscription in subscriptions)
		{
			if (subscription.Inner.State != SubscriptionState.Enabled) continue;
			yield return subscription;
		}
	}
}
