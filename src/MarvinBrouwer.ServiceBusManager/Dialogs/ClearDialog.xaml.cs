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
using MarvinBrouwer.ServiceBusManager.Azure;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;
/// <summary>
/// Interaction logic for ClearDialog.xaml
/// </summary>
public partial class ClearDialog : Page
{
	public ClearDialog()
	{
		InitializeComponent();
	}
	public ClearDialog(int itemCount, bool maxItemsReached)
	{
		InitializeComponent();

		MainQuery.Text = string.Format(MainQuery.Text, itemCount);
		if (!maxItemsReached)
		{
			ItemMaxWarning.Visibility = Visibility.Collapsed;
		}
		else
		{
			ItemMaxWarningMessage.Text = string.Format(ItemMaxWarningMessage.Text, AzureConstants.ServiceBusResourceMaxItemCount);
		}
	}
}
