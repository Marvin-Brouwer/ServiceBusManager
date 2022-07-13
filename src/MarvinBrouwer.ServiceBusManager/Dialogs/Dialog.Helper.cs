using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using MarvinBrouwer.ServiceBusManager.Components;

namespace MarvinBrouwer.ServiceBusManager.Dialogs;

public partial class Dialog
{
	internal static (bool requeue, bool download) ConfirmRequeue(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Requeue items from `{resourceName}`?", "Download before requeue?",
			new RequeueDialog(item is TopicSubscriptionDeadLetterTreeViewItem, itemCount, maxItemsReached));

		var requeue = dialog.ShowDialog() ?? false;
		return (requeue, dialog.DialogBar.StoreBeforeAction);
	}

	internal static (bool requeue, bool download) ConfirmUpload(BaseTreeViewItem item, string fileName, int itemCount)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Upload `{fileName}` to `{resourceName}`?", "Download before requeue?",
			new UploadDialog(item is TopicTreeViewItem, itemCount));

		var requeue = dialog.ShowDialog() ?? false;
		return (requeue, dialog.DialogBar.StoreBeforeAction);
	}

	internal static (bool requeue, bool download) ConfirmClear(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Clear items from `{resourceName}`?", "Download before requeue?",
			new ClearDialog(itemCount, maxItemsReached));

		var requeue = dialog.ShowDialog() ?? false;
		return (requeue, dialog.DialogBar.StoreBeforeAction);
	}

	internal static (bool requeue, bool download) ConfirmDownload(BaseTreeViewItem item, int itemCount, bool maxItemsReached)
	{
		var resourceName = FormatTitle(item);
		var dialog = new Dialog($"Download items from `{resourceName}`?", "Download before requeue?",
			new DownloadDialog(itemCount, maxItemsReached));

		var requeue = dialog.ShowDialog() ?? false;
		return (requeue, dialog.DialogBar.StoreBeforeAction);
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
