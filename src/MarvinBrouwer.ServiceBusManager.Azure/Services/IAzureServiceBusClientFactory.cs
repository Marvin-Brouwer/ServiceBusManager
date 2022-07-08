using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.ServiceBus.Management;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusClientFactory
{
	Task<ManagementClient> GetManagementClient(string secret, CancellationToken cancellationToken);
	Task<ServiceBusClient> GetServiceBusClient(string secret, CancellationToken cancellationToken);

	ServiceBusSender CreateServiceBusSender(MessageHandler resourceData);
	ServiceBusSender CreateServiceBusSender(ServiceBusResource selectedResource, ServiceBusClient client);
}