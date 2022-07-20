using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <summary>
/// Implementation of a <see cref="TreeViewItem"/> containing some properties for button state management
/// and labeling access
/// </summary>
public abstract class BaseTreeViewItem : TreeViewItem
{
	/// <summary>
	/// The plain text display name used in the tree view
	/// </summary>
	public string DisplayName { get; protected init; } = string.Empty;

	/// <summary>
	/// Indicating this item allows for reloading the selected item
	/// </summary>
	public abstract bool CanReload { get; }
	/// <summary>
	/// Indicating this item allows for clearing the selected item
	/// </summary>
	public abstract bool CanClear { get; }
	/// <summary>
	/// Indicating this item allows for uploading messages to the selected item
	/// </summary>
	public abstract bool CanUpload { get; }
	/// <summary>
	/// Indicating this item allows for downloading messages from the selected item
	/// </summary>
	public abstract bool CanDownload { get; }
	/// <summary>
	/// Indicating this item allows for requeueing items from selected item to a designated parent
	/// </summary>
	public abstract bool CanRequeue { get; }

	/// <summary>
	/// An additional label to show in gray next to the tree item
	/// </summary>
	protected string? Label { get; init; }

	/// <summary>
	/// An Icon to use in the tree item
	/// </summary>
	protected string? IconUrl { get; init; }

	/// <summary>
	/// The unique identifier used for this tree item
	/// </summary>
	protected string Identifier
	{
		get => Name;
		set => Name = value;
	}

	/// <inheritdoc cref="BaseTreeViewItem"/>
	protected internal BaseTreeViewItem()
	{
		IsEnabled = false;
		FontWeight = FontWeights.Normal;

		Loaded += (_, _) => SetHeaderValue();
		IsEnabledChanged += (_, _) => SetHeaderValue();
	}

	/// <summary>
	/// Update the header value display
	/// </summary>
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

