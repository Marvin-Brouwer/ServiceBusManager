using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;

using System.Runtime.CompilerServices;

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
			.AuthenticateDefaultTenant(cancellationToken);

		var subscriptions = await azureAuthentication
			.Subscriptions
			.ListAsync(true, cancellationToken);
		
		foreach (var subscription in subscriptions)
		{
			if (subscription.Inner.State != SubscriptionState.Enabled) continue;

			yield return subscription;
		}

		var tenants = await azureAuthentication.Tenants.ListAsync(true, cancellationToken);
		foreach (var tenant in tenants.Where(tenant => tenant.TenantId != azureAuthentication.TenantId))
		{
			var tenantAuthentication = await _authenticationService.Authenticate(tenant, cancellationToken);

			var tenantSubscriptions = await tenantAuthentication
				.Subscriptions
				.ListAsync(true, cancellationToken);

			foreach (var subscription in tenantSubscriptions)
			{
				if (subscription.Inner.State != SubscriptionState.Enabled) continue;

				yield return subscription;
			}
		}
	}
}
