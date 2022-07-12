using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Components;

internal sealed class TopicSubscriptionTreeViewItem : BaseTreeViewItem
{
	public TopicSubscriptionTreeViewItem(TopicSubscription topicSubscription)
	{
		DisplayName = topicSubscription.InnerResource.Name;
		Identifier = $"ID{new Guid(topicSubscription.InnerResource.Key):N}";
		IsEnabled = true;
		SetHeaderValue();

		TopicSubscription = topicSubscription;

		Items.Add(new TopicSubscriptionDeadLetterTreeViewItem(topicSubscription));
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => false;
	public override bool CanDownload => true;
	public override bool CanRequeue => false;

	public TopicSubscription TopicSubscription { get; }

	private sealed class TopicSubscriptionDeadLetterTreeViewItem : BaseTreeViewItem
	{
		public TopicSubscriptionDeadLetterTreeViewItem(TopicSubscription topicSubscription)
		{
			DisplayName = AzureConstants.DeadLetterPathSegment;
			Identifier = $"ID{new Guid(topicSubscription.InnerResource.Key):N}_dl";
			IsEnabled = true;
			SetHeaderValue();

			TopicSubscription = topicSubscription;
			DeadLetter = topicSubscription.DeadLetter;
		}

		public override bool CanReload => false;
		public override bool CanClear => true;
		public override bool CanUpload => false;
		public override bool CanDownload => true;
		public override bool CanRequeue => true;

		public TopicSubscription TopicSubscription { get; }
		public TopicSubscriptionDeadLetter DeadLetter { get; }
	}
}