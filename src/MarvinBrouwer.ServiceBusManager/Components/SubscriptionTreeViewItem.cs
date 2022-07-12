using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Components;

internal sealed class SubscriptionTreeViewItem : BaseTreeViewItem
{
	public SubscriptionTreeViewItem(ISubscription subscription)
	{
		DisplayName = subscription.DisplayName;
		Identifier = $"ID{new Guid(subscription.SubscriptionId):N}";
		SetHeaderValue();

		FontWeight = FontWeights.Bold;
		Subscription = subscription;
	}

	public override bool CanReload => true;
	public override bool CanClear => false;
	public override bool CanUpload => false;
	public override bool CanDownload => false;
	public override bool CanRequeue => false;

	public ISubscription Subscription { get; }
}
