using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record ServiceBus(IServiceBusNamespace InnerResource) : AzureResource<IServiceBusNamespace>(InnerResource, InnerResource)
{
}