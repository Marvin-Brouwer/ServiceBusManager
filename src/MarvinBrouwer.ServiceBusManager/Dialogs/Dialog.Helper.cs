using MarvinBrouwer.ServiceBusManager.Components;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;

public partial class Dialog
{
	private static bool IsResourceWithSubscription(BaseTreeViewItem item) => item
		is TopicTreeViewItem
		or TopicSubscriptionTreeViewItem
		or TopicSubscriptionDeadLetterTreeViewItem;

	internal static (bool requeue, bool storeDownload) ConfirmRequeue(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Requeue items from `{resourceName}`?", "Store locally before requeue?",
			new RequeueDialog(IsResourceWithSubscription(item), itemCount, maxItemsReached));

		var requeue = dialog.ShowDialog() ?? false;
		return (requeue, dialog.DialogBar.StoreBeforeAction);
	}

	internal static bool ConfirmUpload(BaseTreeViewItem item, string fileName, int itemCount)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Upload `{fileName}` to `{resourceName}`?",
			new UploadDialog(IsResourceWithSubscription(item), itemCount));

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

	internal static bool ConfirmReload()
	{
		var dialog = new Dialog($"Reload all subscriptions?", new ReloadDialog());

		return dialog.ShowDialog() ?? false;
	}

	private static string FormatTitle(BaseTreeViewItem item)
	{
		return item switch
		{
			QueueDeadLetterTreeViewItem deadLetter =>
				deadLetter.Queue.Name + "/" +
				ApplicationConstants.DeadLetterPathSegment,
			TopicSubscriptionDeadLetterTreeViewItem deadLetter =>
				deadLetter.Topic.Name + "/" +
				deadLetter.TopicSubscription.Name + "/" +
				ApplicationConstants.DeadLetterPathSegment,
			_ => item.DisplayName
		};
	}
}
