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

internal sealed class ServiceBusTreeViewItem : BaseTreeViewItem
{
	public ServiceBusTreeViewItem(ServiceBus serviceBus)
	{
		DisplayName = serviceBus.InnerResource.Name;
		Identifier = $"ID{new Guid(serviceBus.InnerResource.Key):N}";

		ServiceBus = serviceBus;
	}

	public override bool CanClear => false;
	public override bool CanUpload => false;
	public override bool CanDownload => false;
	public override bool CanRequeue => false;

	public ServiceBus ServiceBus { get; }

	protected override UIElement RenderHeader()
	{
		var mainLabel = base.RenderHeader();
		var subLabel = new Label
		{
			Content = " | " + ServiceBus.InnerResource.ResourceGroupName,
			HorizontalAlignment = HorizontalAlignment.Right,
			HorizontalContentAlignment = HorizontalAlignment.Right,
			FontStyle = FontStyles.Italic,
			Foreground = SystemColors.ActiveBorderBrush,
			Padding = new Thickness(0)
		};
		return new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Children = { mainLabel, subLabel }
		};
	}
}
