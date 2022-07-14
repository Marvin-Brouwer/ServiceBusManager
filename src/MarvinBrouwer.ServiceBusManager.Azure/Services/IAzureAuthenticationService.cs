using Azure.Core;
using Azure.Identity;
using Microsoft.Azure;
using Microsoft.Rest;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureAuthenticationService
{
	DefaultAzureCredential AzureCredentials { get; }

	Task <AccessToken> GetAccessToken(CancellationToken cancellationToken);
	Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> Authenticate(
		CancellationToken cancellationToken);
}