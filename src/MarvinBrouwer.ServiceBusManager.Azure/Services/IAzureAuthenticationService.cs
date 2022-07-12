namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureAuthenticationService
{
	Task<Microsoft.Azure.Management.Fluent.Azure.IAuthenticated> Authenticate(
		CancellationToken cancellationToken);
}