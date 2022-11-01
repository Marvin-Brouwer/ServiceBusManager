using Microsoft.Azure.Management.ResourceManager.Fluent;

using System;
using System.Windows;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <inheritdoc />
public sealed class AzureSubscriptionTreeViewItem : AzureResourceTreeViewItem
{
	internal AzureSubscriptionTreeViewItem(ISubscription subscription) : base(subscription)
	{
		DisplayName = subscription.DisplayName;
		Label = $"tenant: {subscription.Inner.TenantId}";
		Identifier = $"ID{new Guid(subscription.SubscriptionId):N}";
		SetHeaderValue();

		FontWeight = FontWeights.Bold;
		Subscription = subscription;
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
	/// This item's Subscription
	/// </summary>
	public ISubscription Subscription { get; }
}
