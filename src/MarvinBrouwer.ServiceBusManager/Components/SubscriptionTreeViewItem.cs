using System;
using System.Windows;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Components;

public sealed class SubscriptionTreeViewItem : BaseTreeViewItem
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
