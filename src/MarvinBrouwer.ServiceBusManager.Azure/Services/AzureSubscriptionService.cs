using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Azure.Management.Subscription;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using SubscriptionClient = Microsoft.WindowsAzure.Subscriptions.SubscriptionClient;

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
