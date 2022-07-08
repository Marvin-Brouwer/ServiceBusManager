namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public abstract record ServiceBusResource(ServiceBus ServiceBus, string Name)
{
	public virtual string Path => Name;
		
	public abstract ServiceBusResource GetServiceBusTarget();
}