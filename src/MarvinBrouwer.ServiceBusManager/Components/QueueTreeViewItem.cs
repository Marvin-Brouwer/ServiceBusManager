using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

using System;

namespace MarvinBrouwer.ServiceBusManager.Components;

internal sealed class QueueTreeViewItem : ResourceTreeViewItem
{
	public QueueTreeViewItem(Queue queue) : base(queue)
	{
		DisplayName = queue.InnerResource.Name;
		Label = nameof(queue);
		IconUrl = "/Resources/Icons/queue.png";

		Identifier = $"ID{new Guid(queue.InnerResource.Key):N}";
		SetHeaderValue();
		IsEnabled = true;
		
		Items.Add(new QueueDeadLetterTreeViewItem(queue));
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => true;
	public override bool CanDownload => true;
	public override bool CanRequeue => false;
}

internal sealed class QueueDeadLetterTreeViewItem : ResourceTreeViewItem
{
	public QueueDeadLetterTreeViewItem(Queue queue) : base(queue)
	{
		DisplayName = ApplicationConstants.DeadLetterPathSegment;
		IconUrl = "/Resources/Icons/dead-letter.png";

		Identifier = $"ID{new Guid(queue.InnerResource.Key):N}_dl";
		IsEnabled = true;
		SetHeaderValue();

		Queue = queue;
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => false;
	public override bool CanDownload => true;
	public override bool CanRequeue => true;

	public Queue Queue { get; }
}
