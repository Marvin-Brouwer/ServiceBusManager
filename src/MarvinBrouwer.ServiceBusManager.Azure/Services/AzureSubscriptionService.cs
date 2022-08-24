using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;

using System.Runtime.CompilerServices;
using Microsoft.Azure.Management.Fluent;
using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureSubscriptionService : IAzureSubscriptionService
{
	private readonly IAzureAuthenticationService _authenticationService;

	/// <inheritdoc cref="AzureSubscriptionService" />
	public AzureSubscriptionService(IAzureAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<IAzureSubscription> ListSubscriptions([EnumeratorCancellation] CancellationToken cancellationToken)
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

		// It may seem counter-intuitive that we need to re-authenticate for each tenant.
		// However, if you realize what's happening it makes sense.
		// The fact that the azure authentication is able to list all tenants is nice for this application but at the end
		// these re just Id's.
		// If you were to list subscriptions for any of the other tenants, you'll get the same subscriptions as the tenant
		// you're currently authenticated with.
		// This is because the way azure works is you authenticate a tenant (in other words you log in) and you get a token
		// for this specific tenant. So once you're authenticated that Id means nothing for listing subscriptions, it's based on the token.
		// Furthermore, if you were to interact with the service bus with the incorrect token and tenant match you do get weird authentication
		// errors. Because of that we use the same authentication mechanic later on, using the tenant Id to get the correct token.
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
