using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for connecting to the Azure API
/// </summary>
public interface IAzureConnectionService
{
	/// <summary>
	/// Return a list of authenticated connections per <see cref="ITenant"/>
	/// </summary>
	IAsyncEnumerable<AzureConnection> ConnectToAllTenants(CancellationToken cancellationToken);

	/// <summary>
	/// Refresh the connection (re-authenticate and reconnect)
	/// </summary>
	Task<AzureConnection> RefreshConnection(AzureConnection connection, CancellationToken cancellationToken);
}