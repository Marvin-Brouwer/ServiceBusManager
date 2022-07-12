using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ServiceBus.Fluent;

public sealed record TopicSubscriptionDeadLetter(TopicSubscription TopicSubscription) : AzureResource<ISubscription>(TopicSubscription.InnerResource)
{
}