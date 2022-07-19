using Azure.Core;
using Azure.Identity;

using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Subscription;
using Microsoft.Rest;

using IAuthenticated = Microsoft.Azure.Management.Fluent.Azure.IAuthenticated;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureAuthenticationService : IAzureAuthenticationService
{
	private static DefaultAzureCredential InitialAzureCredentials => new(new DefaultAzureCredentialOptions
	{
		ExcludeInteractiveBrowserCredential = false,
		ExcludeEnvironmentCredential = false,
		ExcludeManagedIdentityCredential = false,
		ExcludeSharedTokenCacheCredential = false,
		ExcludeVisualStudioCredential = false,

		ExcludeAzureCliCredential = true,
		ExcludeAzurePowerShellCredential = true,
		ExcludeVisualStudioCodeCredential = true
	});

	private static TokenCredential CreateTenantCredential(string tenantId)
	{
		var tenantEnvironmentCredential = new EnvironmentCredential(new TokenCredentialOptions()
		{
			AuthorityHost = new Uri($"https://login.microsoftonline.com/{tenantId}")
		});
		var managedTenantIdentityCredential = new ManagedIdentityCredential(tenantId);
		var tenantVisualStudioCredential = new VisualStudioCredential(new VisualStudioCredentialOptions { TenantId = tenantId });
		var tenantInteractiveBrowserCredential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
		{
			AuthorityHost = new Uri($"https://login.microsoftonline.com/{tenantId}")
		});

		return new ChainedTokenCredential(
			tenantEnvironmentCredential, managedTenantIdentityCredential,
			tenantVisualStudioCredential, tenantInteractiveBrowserCredential);
	}

	private static async Task<AccessToken> GetAccessToken(CancellationToken cancellationToken)
	{
		return await InitialAzureCredentials
			.GetTokenAsync(new TokenRequestContext(
				new[]
				{
					"https://management.azure.com"
				}), cancellationToken);
	}

	private static async Task<AccessToken> GetAccessToken(string tenantId, CancellationToken cancellationToken)
	{
		return await CreateTenantCredential(tenantId)
			.GetTokenAsync(new TokenRequestContext(
				new[]
				{
					"https://management.azure.com"
				}), cancellationToken);
	}

	/// <inheritdoc />
	public async Task<IAuthenticated> AuthenticateDefaultTenant(CancellationToken cancellationToken)
	{
		var defaultCloudToken = await GetAccessToken(cancellationToken);
		var tokenCredentials = new TokenCredentials(defaultCloudToken.Token);

		var subscriptionClient = new Microsoft.Azure.Management.Subscription.SubscriptionClient(tokenCredentials);
		var tenants = await subscriptionClient.Tenants.ListAsync(cancellationToken);
		var mainTenant = tenants.FirstOrDefault();
		if (mainTenant is null) throw new NotSupportedException("The primary tenant cannot be null");


		var azureCredentials = new AzureCredentials(
			tokenCredentials,
			tokenCredentials,
			mainTenant.TenantId,
			AzureEnvironment.AzureGlobalCloud);

		return Microsoft.Azure.Management.Fluent.Azure
			.Configure()
			.WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
			.Authenticate(azureCredentials);
	}

	/// <inheritdoc />
	public Task<IAuthenticated> Authenticate(ITenant tenant, CancellationToken cancellationToken) =>
		Authenticate(tenant.TenantId, cancellationToken);

	/// <inheritdoc />
	public Task<IAuthenticated> Authenticate(ISubscription subscription, CancellationToken cancellationToken) =>
		Authenticate(subscription.Inner.TenantId, cancellationToken);

	private static async Task<IAuthenticated> Authenticate(string tenantId,CancellationToken cancellationToken)
	{
		var accessToken = await GetAccessToken(tenantId, cancellationToken);
		var tokenCredentials = new TokenCredentials(accessToken.Token);

		var subscriptionClient = new Microsoft.Azure.Management.Subscription.SubscriptionClient(tokenCredentials);
		var tenants = await subscriptionClient.Tenants.ListAsync(cancellationToken);

		var selectedTenant = tenants.FirstOrDefault(listedTenant =>
			string.Equals(listedTenant.TenantId, tenantId, StringComparison.Ordinal));
		if (selectedTenant is null) throw new NotSupportedException("The tenant cannot be null");


		var azureCredentials = new AzureCredentials(
			tokenCredentials,
			tokenCredentials,
			selectedTenant.TenantId,
			AzureEnvironment.AzureGlobalCloud);

		return Microsoft.Azure.Management.Fluent.Azure
			.Configure()
			.WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
			.Authenticate(azureCredentials);
	}
}
