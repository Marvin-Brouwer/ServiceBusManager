using MarvinBrouwer.ServiceBusManager.Azure.Models;

using System;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <inheritdoc />
public sealed class ServiceBusTreeViewItem : ServiceBusResourceTreeViewItem
{
	internal ServiceBusTreeViewItem(ServiceBus serviceBus) : base(serviceBus)
	{
		DisplayName = serviceBus.Name;
		Label = serviceBus.ResourceGroupName;
		IconUrl = "/Resources/Icons/servicebus.png";

		Identifier = $"ID{new Guid(serviceBus.Key):N}";

		ServiceBus = serviceBus;
	}

	/// <inheritdoc />
	public override bool CanReload => true;
	/// <inheritdoc />
	public override bool CanClear => false;
	/// <inheritdoc />
	public override bool CanUpload => false;
	/// <inheritdoc />
	public override bool CanDownload => false;
	/// <inheritdoc />
	public override bool CanRequeue => false;

	/// <summary>
	/// This item's ServiceBus
	/// </summary>
	public ServiceBus ServiceBus { get; }
}
