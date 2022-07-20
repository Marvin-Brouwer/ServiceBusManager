using MarvinBrouwer.ServiceBusManager.Azure;

using System.Windows;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;
/// <summary>
/// Interaction logic for RequeueDialog.xaml
/// </summary>
public partial class RequeueDialog : Page
{
	/// <inheritdoc />
	public RequeueDialog()
	{
		InitializeComponent();
	}
	/// <inheritdoc />
	public RequeueDialog(bool isSubscription, int itemCount, bool maxItemsReached)
	{
		InitializeComponent();

		MainQuery.Text = string.Format(MainQuery.Text, itemCount);
		if (!isSubscription) SubscriptionWarning.Visibility = Visibility.Collapsed;
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
