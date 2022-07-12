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

internal sealed class QueueTreeViewItem : BaseTreeViewItem
{
	public QueueTreeViewItem(Queue queue)
	{
		DisplayName = queue.InnerResource.Name;
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

	private sealed class QueueDeadLetterTreeViewItem : BaseTreeViewItem
	{
		public QueueDeadLetterTreeViewItem(Queue queue)
		{
			DisplayName = AzureConstants.DeadLetterPathSegment;
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
}
