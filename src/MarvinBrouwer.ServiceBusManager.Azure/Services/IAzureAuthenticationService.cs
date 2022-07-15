using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureAuthenticationService
{
	Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> AuthenticateDefaultTenant(
		CancellationToken cancellationToken);

	Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> Authenticate(
		ITenant tenant, CancellationToken cancellationToken);
	Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> Authenticate(
		ISubscription subscription, CancellationToken cancellationToken);
}