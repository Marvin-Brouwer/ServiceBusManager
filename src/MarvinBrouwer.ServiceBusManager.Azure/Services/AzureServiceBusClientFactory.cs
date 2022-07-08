using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus.Management;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureServiceBusClientFactory : IAzureServiceBusClientFactory
{
	private readonly KeyVaultClient _keyVaultClient;

	public AzureServiceBusClientFactory(KeyVaultClient keyVaultClient)
	{
		_keyVaultClient = keyVaultClient;
	}
	
	public async Task<ManagementClient> GetManagementClient(string secret, CancellationToken cancellationToken)
	{
		var connectionString = await _keyVaultClient.GetSecretAsync(secret, cancellationToken);
		return new ManagementClient(connectionString.Value);
	}

	public async Task<ServiceBusClient> GetServiceBusClient(string secret, CancellationToken cancellationToken)
	{
		var connectionString = await _keyVaultClient.GetSecretAsync(secret, cancellationToken);
		return new ServiceBusClient(connectionString.Value);
	}

	public ServiceBusSender CreateServiceBusSender(MessageHandler resourceData) => CreateServiceBusSender(resourceData.Resource, resourceData.Client);

	public ServiceBusSender CreateServiceBusSender(ServiceBusResource selectedResource, ServiceBusClient client)
	{
		var requeueTarget = selectedResource.GetServiceBusTarget();
		if (requeueTarget is Queue || requeueTarget is Topic)
			return client.CreateSender(requeueTarget.Path);

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

}