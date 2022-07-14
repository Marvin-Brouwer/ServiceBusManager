using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Subscription;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Rest;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureAuthenticationService : IAzureAuthenticationService
{
	public DefaultAzureCredential AzureCredentials => new(new DefaultAzureCredentialOptions
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

	public async Task<AccessToken> GetAccessToken(CancellationToken cancellationToken)
	{
		var claims = new List<string>
		{
			new Claim("Manage", "true").ToString(),
			new Claim("Send", "true").ToString(),
			new Claim("Listen", "true").ToString(),
		};
		return await AzureCredentials
			.GetTokenAsync(new TokenRequestContext(
				new[]
				{
					"https://management.azure.com"
				},
				null,
				string.Join(",", claims)), cancellationToken);
	}

	public async Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> Authenticate(
		CancellationToken cancellationToken)
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
}
