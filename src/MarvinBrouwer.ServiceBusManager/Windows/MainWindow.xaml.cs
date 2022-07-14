using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;
using MarvinBrouwer.ServiceBusManager.Dialogs;
using MarvinBrouwer.ServiceBusManager.Services;
using MdXaml;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using TreeView = System.Windows.Controls.TreeView;

namespace MarvinBrouwer.ServiceBusManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly IAzureServiceBusResourceQueryService _resourceQueryService;
	private readonly IAzureAuthenticationService _azureAuthenticationService;
	private readonly AzureLandscapeRenderingService _azureLandscapeRenderingService;
	private readonly LocalStorageService _localStorageService;

	private CancellationToken CancellationToken => ((App) Application.Current).CancellationToken;

	public MainWindow(
		IAzureServiceBusResourceQueryService resourceQueryService,
		IAzureAuthenticationService azureAuthenticationService,
		Func<TreeView, AzureLandscapeRenderingService> azureLandscapeRenderingService,
		LocalStorageService localStorageService)
	{
		InitializeComponent();

		_resourceQueryService = resourceQueryService;
		_azureAuthenticationService = azureAuthenticationService;
		_azureLandscapeRenderingService = azureLandscapeRenderingService(AzureLandscape);
		_localStorageService = localStorageService;

		InitializeWindow();
	}

	private void InitializeWindow()
	{
		ClearStatusPanel();
		AppendStatusMessage(@"Loading data...");
		SelectedItemChanged(null);

		Closing += (_, _) => ((App)Application.Current).SignalCancel();
		Cursor = Cursors.Wait;
		MenuItemReload.IsEnabled = false;

		Loaded += (_, _) => WindowLoaded();
	}

	#region WindowUtilities
	private void AppendStatusMessage(string message)
	{
		var paddingTop = StatusBox.Children.Count == 0 ? 10 : 0;
		StatusBox.Children.Add(new TextBlock {
			Text = string.IsNullOrWhiteSpace(message) ? Environment.NewLine : message,
			Padding = new Thickness(10, paddingTop, 10, 2)
		});
	}

	private void ClearStatusPanel()
	{
		StatusBox.Children.Clear();
	}
	#endregion

	private async void WindowLoaded()
	{
		await _azureAuthenticationService.Authenticate(CancellationToken);
		_localStorageService.PrepareDownloadFolder();
		Cursor = Cursors.Arrow;
		await LoadFullAzureLandscape();
	}

	private async void ReloadAzureLandscape(object sender, RoutedEventArgs e)
	{
		ClearStatusPanel();
		AppendStatusMessage(@"Loading data...");
		await LoadFullAzureLandscape();
	}

	private async Task LoadFullAzureLandscape()
	{
		AzureLandscape.IsEnabled = false;
		SelectedItemChanged(null);
		foreach (SubscriptionTreeViewItem treeViewItem in AzureLandscape.Items)
		{
			treeViewItem.IsEnabled = false;
			treeViewItem.IsExpanded = false;
		}
		AzureLandscape.IsEnabled = true;
		CheckAzureLandscapeState();

		var subscriptionTreeViewItems = _azureLandscapeRenderingService.LoadSubscriptions(CancellationToken);
		await foreach (var subscriptionTreeViewItem in subscriptionTreeViewItems.WithCancellation(CancellationToken))
		{
			subscriptionTreeViewItem.IsEnabledChanged += (_, _) => CheckAzureLandscapeState();
		}

		CheckAzureLandscapeState();
	}

	private bool AzureLandscapeIsLoading =>
		AzureLandscape.Items.Count == 0 ||
		!AzureLandscape.Items.Cast<SubscriptionTreeViewItem>().All(treeViewItem => treeViewItem.IsEnabled);

	private void CheckAzureLandscapeState()
	{
		AzureLandscape.Cursor = AzureLandscapeIsLoading ? Cursors.AppStarting : Cursors.Arrow;
		MenuItemReload.IsEnabled = !AzureLandscapeIsLoading;

		if (!AzureLandscapeIsLoading) AppendStatusMessage(@"Done!");
	}

	private async void ReloadItem(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is null) return;
		if (AzureLandscape.SelectedItem is not BaseTreeViewItem { CanReload: true } baseTreeViewItem) return;

		baseTreeViewItem.IsEnabled = false;
		baseTreeViewItem.IsExpanded = false;

		if (AzureLandscape.SelectedItem is SubscriptionTreeViewItem subscriptionTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadSubscriptionContents(subscriptionTreeViewItem, CancellationToken);
		}

		if (AzureLandscape.SelectedItem is ServiceBusTreeViewItem serviceBusTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadServiceBusResources(serviceBusTreeViewItem, CancellationToken);
		}

		if (AzureLandscape.SelectedItem is TopicTreeViewItem topicTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadTopicSubscriptions(topicTreeViewItem, CancellationToken);
		}
	}

	private void OpenDownloadFolder(object sender, RoutedEventArgs e)
	{
		_localStorageService.OpenDownloadFolder();
	}

	private void OpenGitHubRepository(object sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = "https://github.com/Marvin-Brouwer/ServiceBusManager",
			Verb = "open",
			UseShellExecute = true
		});
	}

	private void ShowReadmeWindow(object sender, RoutedEventArgs e)
	{
		if (ReadmeRenderer.Source is null)
		{
			// This is probably not efficient but we couldn't find a better way.
			ReadmeRenderer.Source = new Uri("pack://application:,,,./Readme.md");

			var resource = App.GetResourceStream(ReadmeRenderer.Source);
			using var reader = new StreamReader(resource!.Stream);

			ReadmeRenderer.Markdown = reader.ReadToEnd(); ;
		}
		HelpPanel.Visibility = Visibility.Visible;
	}

	private void HideReadmeWindow(object sender, RoutedEventArgs e)
	{
		HelpPanel.Visibility = Visibility.Collapsed;
	}

	private static (int fullCount, bool maxItemsReached) ValidateItemCount(long fullCount)
	{
		if (fullCount <= AzureConstants.ServiceBusResourceMaxItemCount)
			return ((int)fullCount, false);

		return (AzureConstants.ServiceBusResourceMaxItemCount, true);
	}

	private async void ShowRequeueDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanRequeue) return;

		ClearStatusPanel();
		AppendStatusMessage("Getting item count to requeue");
		var fullCount = await _resourceQueryService.GetMessageCount(treeViewItem.Resource, CancellationToken);
		var (itemCount, maxItemsReached) = ValidateItemCount(fullCount);

		var (requeue, storeDownload) = Dialog.ConfirmRequeue(treeViewItem, itemCount, maxItemsReached);
		if (!requeue)
		{
			AppendStatusMessage("Canceled");
			return;
		}
		
		AppendStatusMessage("Downloading message data (peek)");
		var serviceBusMessages = await _resourceQueryService
			.ReadAllMessages(treeViewItem.Resource, CancellationToken);

		if (storeDownload)
		{
			// TODO STORE
		}

		// TODO PUSH
		throw new System.NotImplementedException();
	}

	private async void ShowDownloadDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanDownload) return;

		ClearStatusPanel();
		AppendStatusMessage("Getting item count to download");
		var fullCount = await _resourceQueryService.GetMessageCount(treeViewItem.Resource, CancellationToken);
		var (itemCount, maxItemsReached) = ValidateItemCount(fullCount);

		var download = Dialog.ConfirmDownload(treeViewItem, itemCount, maxItemsReached);
		if (!download)
		{
			AppendStatusMessage("Canceled");
			return;
		}

		AppendStatusMessage("Downloading message data (peek)");
		var serviceBusMessages = await _resourceQueryService
			.ReadAllMessages(treeViewItem.Resource, CancellationToken);
		
		// TODO STORE
		throw new System.NotImplementedException();
	}

	private async void ShowUploadDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanUpload) return;

		ClearStatusPanel();
		AppendStatusMessage("Selecting file(s) to upload");

		// TODO select, unpack, count

		var upload = Dialog.ConfirmUpload(treeViewItem, "todo filename.ext" /* todo */, 99 /*todo*/);
		if (!upload)
		{
			AppendStatusMessage("Canceled");
			return;
		}

		// TODO PUSH
		throw new System.NotImplementedException();
	}

	private async void ShowClearDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanClear) return;

		ClearStatusPanel();
		AppendStatusMessage("Getting item count to clear");
		var fullCount = await _resourceQueryService.GetMessageCount(treeViewItem.Resource, CancellationToken);
		var (itemCount, maxItemsReached) = ValidateItemCount(fullCount);

		var (clear, storeDownload) = Dialog.ConfirmClear(treeViewItem, itemCount, maxItemsReached);
		if (!clear)
		{
			AppendStatusMessage("Canceled");
			return;
		}

		AppendStatusMessage("Downloading message data (peek)");
		var serviceBusMessages = await _resourceQueryService
			.ReadAllMessages(treeViewItem.Resource, CancellationToken);

		if (storeDownload)
		{
			// TODO STORE
		}


		// TODO CLEAR
		throw new System.NotImplementedException();
	}

	private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (AzureLandscape.SelectedItem is null)
			SelectedItemChanged(null);
		else if (AzureLandscape.SelectedItem is BaseTreeViewItem treeViewItem)
			SelectedItemChanged(treeViewItem);
		else SelectedItemChanged(null);
	}

	private void SelectedItemChanged(BaseTreeViewItem? treeViewItem)
	{
		ButtonClear.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanClear;
		ButtonDownload.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanDownload;
		ButtonRequeue.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanRequeue;
		ButtonUpload.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanUpload;

		MenuItemReloadSelected.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanReload;

		CommandManager.InvalidateRequerySuggested();
	}
}