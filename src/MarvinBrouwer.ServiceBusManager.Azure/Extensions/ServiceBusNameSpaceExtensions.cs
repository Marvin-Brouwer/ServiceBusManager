using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Extensions;

internal static class ServiceBusNameSpaceExtensions
{
	/// <summary>
	/// Use the <see cref="IServiceBusNamespace"/>s Authorization rules to get
	/// a connection string containing the correct access rights
	/// </summary>
	private static async Task<string> GetAccessConnectionString(
		this IServiceBusNamespace serviceBusNamespace, AccessRights minimalAccessRights, CancellationToken cancellationToken)
	{
		var rootManageSharedAccessKey = await serviceBusNamespace.AuthorizationRules
			.GetByNameAsync("RootManageSharedAccessKey", cancellationToken);

		// Try the default access key first
		if (rootManageSharedAccessKey != null && rootManageSharedAccessKey.Rights.Contains(minimalAccessRights))
			return (await rootManageSharedAccessKey!.GetKeysAsync(cancellationToken)).PrimaryConnectionString;

		// Find one with enough rights
		var accessKeyRecord = (await serviceBusNamespace.AuthorizationRules
				.ListAsync(true, cancellationToken))
			.FirstOrDefault(key => key.Rights.Contains(minimalAccessRights));

		// If you ever run into this.
		// Alternatively you can use the _authenticationService.AzureCredentials to pass to the ServiceBusClient
		// We're not sure if this works, it sure didn't with the RootManageSharedAccessKey in place.
		if (accessKeyRecord is null) throw new NotSupportedException(
			"Currently we don't support service buses without access keys");

		return (await accessKeyRecord.GetKeysAsync(cancellationToken)).PrimaryConnectionString;
	}

	public static async Task<ServiceBusClient> CreateServiceBusClient(
		this IServiceBusNamespace serviceBusNamespace, AccessRights minimalAccessRights, CancellationToken cancellationToken)
	{
		var connectionString = await serviceBusNamespace.GetAccessConnectionString(minimalAccessRights, cancellationToken);

		return new ServiceBusClient(connectionString, new ServiceBusClientOptions
		{
			EnableCrossEntityTransactions = true,
			TransportType = ServiceBusTransportType.AmqpTcp
		});
	}
}
