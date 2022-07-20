using MarvinBrouwer.ServiceBusManager.Azure.Models;

using System;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <inheritdoc />
public sealed class TopicTreeViewItem : ResourceTreeViewItem
{
	internal TopicTreeViewItem(Topic topic) : base(topic)
	{
		DisplayName = topic.InnerResource.Name;
		Label = nameof(topic);
		IconUrl = "/Resources/Icons/topic.png";

		Identifier = $"ID{new Guid(topic.InnerResource.Key):N}";
		SetHeaderValue();

		Topic = topic;
	}

	/// <inheritdoc />
	public override bool CanReload => true;
	/// <inheritdoc />
	public override bool CanClear => false;
	/// <inheritdoc />
	public override bool CanUpload => true;
	/// <inheritdoc />
	public override bool CanDownload => false;
	/// <inheritdoc />
	public override bool CanRequeue => false;

	/// <summary>
	/// This items Topic
	/// </summary>
	public Topic Topic { get; }
}
