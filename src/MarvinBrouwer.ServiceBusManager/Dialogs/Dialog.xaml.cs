using System;
using System.Windows;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;

/// <summary>
/// Interaction logic for Dialog.xaml
/// </summary>
public partial class Dialog : Window
{
	/// <inheritdoc cref="Dialog" />
	protected Dialog()
	{
		InitializeComponent();
	}

	/// <inheritdoc cref="Dialog" />
	private Dialog(
		string title,
		string storeCheckBoxLabel,
		Page dialogPage)
	{
		InitializeComponent();
		Icon = null;
		Title = title;

		DialogBar.CheckBoxLabel = storeCheckBoxLabel;
		DialogBar.OnOk += OnOkClick;
		DialogBar.OnCancel += OnCancelClick;

		ContentPlaceHolder.Content = dialogPage;
	}

	private Dialog(
		string title,
		Page dialogPage)
	{
		InitializeComponent();
		Icon = null;
		Title = title;

		DialogBar.CheckBoxLabel = null;
		DialogBar.OnOk += OnOkClick;
		DialogBar.OnCancel += OnCancelClick;

		ContentPlaceHolder.Content = dialogPage;
	}

	private void OnOkClick(object? sender, EventArgs e)
	{
		DialogResult = true;
	}

	private void OnCancelClick(object? sender, EventArgs e)
	{
		DialogResult = false;
	}
}