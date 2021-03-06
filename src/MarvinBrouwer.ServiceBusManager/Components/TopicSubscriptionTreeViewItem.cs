using MarvinBrouwer.ServiceBusManager.Azure.Models;

using System;


namespace MarvinBrouwer.ServiceBusManager.Components;

internal sealed class TopicSubscriptionTreeViewItem : ResourceTreeViewItem
{
	public TopicSubscriptionTreeViewItem(TopicSubscription topicSubscription, Topic topic) : base(topicSubscription)
	{
		DisplayName = topicSubscription.InnerResource.Name;
		IconUrl = "/Resources/Icons/topic-subscription.png";

		Identifier = $"ID{new Guid(topicSubscription.InnerResource.Key):N}";
		IsEnabled = true;
		SetHeaderValue();

		TopicSubscription = topicSubscription;

		Items.Add(new TopicSubscriptionDeadLetterTreeViewItem(topicSubscription, topic));
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => false;
	public override bool CanDownload => true;
	public override bool CanRequeue => true;

	public TopicSubscription TopicSubscription { get; }
}

internal sealed class TopicSubscriptionDeadLetterTreeViewItem : ResourceTreeViewItem
{
	public TopicSubscriptionDeadLetterTreeViewItem(TopicSubscription topicSubscription, Topic topic) : base(topicSubscription)
	{
		DisplayName = ApplicationConstants.DeadLetterPathSegment;
		IconUrl = "/Resources/Icons/dead-letter.png";

		Identifier = $"ID{new Guid(topicSubscription.InnerResource.Key):N}_dl";
		IsEnabled = true;
		SetHeaderValue();

		Topic = topic;
		TopicSubscription = topicSubscription;
		DeadLetter = topicSubscription.DeadLetter;
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => false;
	public override bool CanDownload => true;
	public override bool CanRequeue => true;

	public Topic Topic { get; }
	public TopicSubscription TopicSubscription { get; }
	public TopicSubscriptionDeadLetter DeadLetter { get; }
}