using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <summary>
/// Interaction logic for DialogBar.xaml
/// </summary>
public partial class DialogBar : UserControl
{
	/// <summary>
	/// Add an event handler to the OK button
	/// </summary>
	[Category("Behavior")] public event EventHandler OnOk = default!;

	/// <summary>
	/// Add an event handler to the Cancel button
	/// </summary>
	[Category("Behavior")] public event EventHandler OnCancel = default!;

	/// <summary>
	/// Get or set the label of this bar's checkbox.
	/// Will be hidden if <c>string.Empty</c> or <c>null</c>.
	/// </summary>
	public string? CheckBoxLabel
	{
		get => StoreBeforeActionCheckbox.Content?.ToString();
		set
		{
			StoreBeforeActionCheckbox.Content = value;
			StoreBeforeActionCheckbox.Visibility = string.IsNullOrWhiteSpace(value)
				? Visibility.Collapsed
				: Visibility.Visible;
		}
	}

	/// <summary>
	/// Get the value of the checkbox
	/// </summary>
	public bool StoreBeforeAction => StoreBeforeActionCheckbox.IsChecked ?? false;

	/// <inheritdoc cref="DialogBar" />
	public DialogBar()
	{
		InitializeComponent();
	}

	private void OnOkClick(object sender, RoutedEventArgs e)
	{
		OnOk(sender, e);
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		OnCancel(sender, e);
	}
}