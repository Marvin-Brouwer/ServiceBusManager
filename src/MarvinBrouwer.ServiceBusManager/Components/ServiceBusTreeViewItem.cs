using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;

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
