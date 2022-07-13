using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;

/// <summary>
/// Interaction logic for RequeueDialog.xaml
/// </summary>
public partial class Dialog : Window
{
	protected Dialog()
	{
		InitializeComponent();
	}

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

	private void OnOkClick(object? sender, EventArgs e)
	{
		DialogResult = true;
	}

	private void OnCancelClick(object? sender, EventArgs e)
	{
		DialogResult = false;
	}
}