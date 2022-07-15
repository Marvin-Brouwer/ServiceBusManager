using MarvinBrouwer.ServiceBusManager.Azure;

using System.Windows;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;
/// <summary>
/// Interaction logic for DownloadDialog.xaml
/// </summary>
public partial class DownloadDialog : Page
{
	public DownloadDialog()
	{
		InitializeComponent();
	}
	public DownloadDialog(int itemCount, bool maxItemsReached)
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
			ItemMaxWarningDesciption.Text = string.Format(ItemMaxWarningDesciption.Text, AzureConstants.ServiceBusResourceMaxItemCount);
		}
	}
}
