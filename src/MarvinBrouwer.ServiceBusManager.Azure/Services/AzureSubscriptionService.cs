using System.Runtime.CompilerServices;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureSubscriptionService : IAzureSubscriptionService
{
	private readonly IAzureAuthenticationService _authenticationService;

	public AzureSubscriptionService(IAzureAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	public async IAsyncEnumerable<ISubscription> ListSubscriptions([EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var azureAuthentication = await _authenticationService
			.Authenticate(cancellationToken);
		var azureConnection = await azureAuthentication
			.WithDefaultSubscriptionAsync();

		var subscriptions = await azureConnection
			.Subscriptions
			.ListAsync(true, cancellationToken);

		foreach (var subscription in subscriptions)
		{
			if (subscription.Inner.State != SubscriptionState.Enabled) continue;

			yield return subscription;
		}
	}
}
