using System;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

namespace MarvinBrouwer.ServiceBusManager.Components;

public sealed class TopicTreeViewItem : ResourceTreeViewItem
{
	public TopicTreeViewItem(Topic topic) : base(topic)
	{
		DisplayName = topic.InnerResource.Name;
		Label = nameof(topic);
		IconUrl = "/Resources/Icons/topic.png";

		Identifier = $"ID{new Guid(topic.InnerResource.Key):N}";
		SetHeaderValue();

		Topic = topic;
	}

	public override bool CanReload => true;
	public override bool CanClear => false;
	public override bool CanUpload => true;
	public override bool CanDownload => false;
	public override bool CanRequeue => false;

	public Topic Topic { get; }
}
