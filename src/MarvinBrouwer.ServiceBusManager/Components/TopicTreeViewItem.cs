using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Components;

internal sealed class TopicTreeViewItem : ResourceTreeViewItem
{
	public TopicTreeViewItem(Topic topic) : base(topic.InnerResource)
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
