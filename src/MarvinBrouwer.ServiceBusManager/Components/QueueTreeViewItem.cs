using System;
using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

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

		Queue = queue;
		
		Items.Add(new QueueDeadLetterTreeViewItem(queue));
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => true;
	public override bool CanDownload => true;
	public override bool CanRequeue => false;

	public Queue Queue { get; }
}

internal sealed class QueueDeadLetterTreeViewItem : ResourceTreeViewItem
{
	public QueueDeadLetterTreeViewItem(Queue queue) : base(queue)
	{
		DisplayName = AzureConstants.DeadLetterPathSegment;
		IconUrl = "/Resources/Icons/dead-letter.png";

		Identifier = $"ID{new Guid(queue.InnerResource.Key):N}_dl";
		IsEnabled = true;
		SetHeaderValue();

		Queue = queue;
		DeadLetter = queue.DeadLetter;
	}

	public override bool CanReload => false;
	public override bool CanClear => true;
	public override bool CanUpload => false;
	public override bool CanDownload => true;
	public override bool CanRequeue => true;

	public Queue Queue { get; }
	public QueueDeadLetter DeadLetter { get; }
}
