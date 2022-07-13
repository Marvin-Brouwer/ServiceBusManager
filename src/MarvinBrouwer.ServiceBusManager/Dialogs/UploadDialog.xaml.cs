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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;
/// <summary>
/// Interaction logic for UploadDialog.xaml
/// </summary>
public partial class UploadDialog : Page
{
	public UploadDialog()
	{
		InitializeComponent();
	}
	public UploadDialog(bool isSubscription, int itemCount)
	{
		InitializeComponent();

		MainQuery.Text = string.Format(MainQuery.Text, itemCount);
		if (!isSubscription) SubscriptionWarning.Visibility = Visibility.Collapsed;
	}
}
