using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <summary>
/// Interaction logic for DialogBar.xaml
/// </summary>
public partial class DialogBar : UserControl
{
	[Category("Behavior")] public event EventHandler OnOk;

	[Category("Behavior")] public event EventHandler OnCancel;

	public string CheckBoxLabel
	{
		get => StoreBeforeActionCheckbox.Content?.ToString() ?? "Save?";
		set => StoreBeforeActionCheckbox.Content = value;
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