using MarvinBrouwer.ServiceBusManager.Azure.Models;

using System;

namespace MarvinBrouwer.ServiceBusManager.Components;

public sealed class ServiceBusTreeViewItem : ResourceTreeViewItem
{
	public ServiceBusTreeViewItem(ServiceBus serviceBus) : base(serviceBus)
	{
		DisplayName = serviceBus.InnerResource.Name;
		Label = serviceBus.InnerResource.ResourceGroupName;
		IconUrl = "/Resources/Icons/servicebus.png";

		Identifier = $"ID{new Guid(serviceBus.InnerResource.Key):N}";

		ServiceBus = serviceBus;
	}

	public override bool CanReload => true;
	public override bool CanClear => false;
	public override bool CanUpload => false;
	public override bool CanDownload => false;
	public override bool CanRequeue => false;

	public ServiceBus ServiceBus { get; }
}
