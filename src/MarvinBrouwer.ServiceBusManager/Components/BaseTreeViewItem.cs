using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MarvinBrouwer.ServiceBusManager.Components;

public abstract class BaseTreeViewItem : TreeViewItem
{
	public string DisplayName { get; init; } = string.Empty;

	public abstract bool CanReload { get; }
	public abstract bool CanClear { get; }
	public abstract bool CanUpload { get; }
	public abstract bool CanDownload { get; }
	public abstract bool CanRequeue { get; }
	public string? Label { get; protected init; }
	public string? IconUrl { get; protected init; }

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
	
	protected void SetHeaderValue()
	{
		var headerTitle = new TextBlock
		{
			Text = IsEnabled ? DisplayName : DisplayName + "...",
			Padding = new Thickness(0)
		};

		var icon = RenderIcon();
		var headerLabel = RenderLabel();

		var stackPanel = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Children = { headerTitle }
		};
		if (icon is not null) stackPanel.Children.Insert(0, icon);
		if (headerLabel is not null) stackPanel.Children.Add(headerLabel);

		Header = stackPanel;
	}

	private UIElement? RenderIcon()
	{
		if (string.IsNullOrWhiteSpace(IconUrl)) return null;

		return new Image
		{
			Source = new BitmapImage(new Uri($"pack://application:,,,{IconUrl}")),
			Width = 12,
			Height = 12,
			Margin = new Thickness(0, 0, 4, 0)
		};
	}

	private UIElement? RenderLabel()
	{
		if (string.IsNullOrWhiteSpace(Label)) return null;
		return new TextBlock
		{
			Text = " " + Label,
			HorizontalAlignment = HorizontalAlignment.Right,
			FontStyle = FontStyles.Italic,
			FontWeight = FontWeights.Normal,
			Foreground = SystemColors.ActiveBorderBrush,
			Padding = new Thickness(0)
		};
	}
}

