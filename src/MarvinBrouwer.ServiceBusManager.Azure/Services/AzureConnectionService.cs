using System.Runtime.CompilerServices;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureConnectionService : IAzureConnectionService
{
	private readonly IAzureAuthenticationService _authenticationService;

	/// <inheritdoc cref="AzureConnectionService" />
	public AzureConnectionService(IAzureAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<AzureConnection> ConnectToAllTenants([EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var azureAuthentication = await _authenticationService
			.AuthenticateDefaultTenant(cancellationToken);

		var defaultTenantId = azureAuthentication.AzureClient.TenantId;
		var tenants = await azureAuthentication
			.AzureClient
			.Tenants
			.ListAsync(true, cancellationToken);

		foreach (var tenant in tenants)
		{
			if (tenant.TenantId == defaultTenantId)
			{
				yield return new AzureConnection(tenant, azureAuthentication);
				continue;
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

			var tenantAuthentication = await _authenticationService.Authenticate(tenant, cancellationToken);

			yield return new AzureConnection(tenant, tenantAuthentication);
		}
	}

	/// <inheritdoc />
	public async Task<AzureConnection> RefreshConnection(AzureConnection connection, CancellationToken cancellationToken)
	{
		var tenantAuthentication = await _authenticationService
			.AuthenticateDefaultTenant(cancellationToken);
		
		var tenants = await tenantAuthentication
			.AzureClient
			.Tenants
			.ListAsync(true, cancellationToken);

		var requestedTenant = tenants.First(tenant => tenant.TenantId == connection.Tenant.TenantId);
		return new AzureConnection(requestedTenant, tenantAuthentication);
	}
}
