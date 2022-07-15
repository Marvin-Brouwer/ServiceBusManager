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
	[Category("Behavior")] public event EventHandler OnOk;

	[Category("Behavior")] public event EventHandler OnCancel;

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

	public bool StoreBeforeAction => StoreBeforeActionCheckbox.IsChecked ?? false;

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