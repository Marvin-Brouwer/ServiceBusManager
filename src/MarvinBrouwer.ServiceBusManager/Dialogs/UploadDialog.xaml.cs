using System.Windows;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;
/// <summary>
/// Interaction logic for UploadDialog.xaml
/// </summary>
public partial class UploadDialog : Page
{
	/// <inheritdoc />
	public UploadDialog()
	{
		InitializeComponent();
	}
	/// <inheritdoc />
	public UploadDialog(bool isSubscription, int itemCount)
	{
		InitializeComponent();

		MainQuery.Text = string.Format(MainQuery.Text, itemCount);
		if (!isSubscription) SubscriptionWarning.Visibility = Visibility.Collapsed;
	}
}
