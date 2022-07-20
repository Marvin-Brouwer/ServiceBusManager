using MarvinBrouwer.ServiceBusManager.Azure;

using System.Windows;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;
/// <summary>
/// Interaction logic for ClearDialog.xaml
/// </summary>
public partial class ClearDialog : Page
{
	/// <inheritdoc cref="ClearDialog" />
	public ClearDialog()
	{
		InitializeComponent();
	}

	/// <inheritdoc cref="ClearDialog" />
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
