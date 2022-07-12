using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Subscription;
using Microsoft.Rest;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureAuthenticationService : IAzureAuthenticationService
{
	private async Task<TokenCloudCredentials> GetAccessToken(CancellationToken cancellationToken)
	{
		// Todo store token locally
		
		var token = await RequestAccessToken(cancellationToken);
		return token;
	}

	private async Task<TokenCloudCredentials> RequestAccessToken(CancellationToken cancellationToken)
	{
		var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
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

		var defaultCloudToken = await azureCredentials
			.GetTokenAsync(new TokenRequestContext(new[] { "https://management.azure.com" }), cancellationToken);

		return new TokenCloudCredentials(defaultCloudToken.Token);
	}

	public async Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> Authenticate(
		CancellationToken cancellationToken)
	{
		var defaultCloudToken = await GetAccessToken(cancellationToken);
		var tokenCredentials = new TokenCredentials(defaultCloudToken.Token);

		var subscriptionClient = new Microsoft.Azure.Management.Subscription.SubscriptionClient(new TokenCredentials(defaultCloudToken.Token));
		var tenants = await subscriptionClient.Tenants.ListAsync(cancellationToken);
		var mainTenant = tenants.FirstOrDefault();
		// todo validate tenant not null

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
}
