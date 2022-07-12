using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Components;

internal abstract class BaseTreeViewItem : TreeViewItem
{
	protected string DisplayName { get; init; } = string.Empty;

	public abstract bool CanClear { get; }
	public abstract bool CanUpload { get; }
	public abstract bool CanDownload { get; }
	public abstract bool CanRequeue { get; }

	protected string Identifier
	{
		get => Name;
		set => Name = value;
	}

	protected BaseTreeViewItem()
	{
		IsEnabled = false;
		FontWeight = FontWeights.Normal;

		Loaded += (_, _) => SetHeaderValue();
		IsEnabledChanged += (_, _) => SetHeaderValue();
	}

	protected virtual UIElement RenderHeader()
	{
		return new Label
		{
			Content = IsEnabled ? DisplayName : DisplayName + "...",
			Padding = new Thickness(0)
		};
	}
	protected void SetHeaderValue()
	{
		// todo icons
		// todo loading icon?
		Header = RenderHeader();
	}
}
