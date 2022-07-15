using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Components;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;

public partial class Dialog
{
	internal static (bool requeue, bool storeDownload) ConfirmRequeue(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Requeue items from `{resourceName}`?", "Store locally before requeue?",
			new RequeueDialog(item is TopicSubscriptionDeadLetterTreeViewItem, itemCount, maxItemsReached));

		var requeue = dialog.ShowDialog() ?? false;
		return (requeue, dialog.DialogBar.StoreBeforeAction);
	}

	internal static bool ConfirmUpload(BaseTreeViewItem item, string fileName, int itemCount)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Upload `{fileName}` to `{resourceName}`?",
			new UploadDialog(item is TopicTreeViewItem, itemCount));

		return dialog.ShowDialog() ?? false;
	}

	internal static (bool clear, bool storeDownload) ConfirmClear(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Clear items from `{resourceName}`?", "Store locally before clear?",
			new ClearDialog(itemCount, maxItemsReached));

		var clear = dialog.ShowDialog() ?? false;
		return (clear, dialog.DialogBar.StoreBeforeAction);
	}

	internal static bool ConfirmDownload(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Download items from `{resourceName}`?",
			new DownloadDialog(itemCount, maxItemsReached));

		return dialog.ShowDialog() ?? false;
	}

	private static string FormatTitle(BaseTreeViewItem item)
	{
		return item switch
		{
			QueueDeadLetterTreeViewItem deadLetter =>
				deadLetter.Queue.InnerResource.Name + "/" +
				AzureConstants.DeadLetterPathSegment,
			TopicSubscriptionDeadLetterTreeViewItem deadLetter =>
				deadLetter.Topic.InnerResource.Name + "/" +
				deadLetter.TopicSubscription.InnerResource.Name + "/" +
				AzureConstants.DeadLetterPathSegment,
			_ => item.DisplayName
		};
	}
}
