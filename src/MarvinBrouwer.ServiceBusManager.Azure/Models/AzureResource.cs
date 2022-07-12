namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public abstract record AzureResource<TResource>(TResource InnerResource)
{
}