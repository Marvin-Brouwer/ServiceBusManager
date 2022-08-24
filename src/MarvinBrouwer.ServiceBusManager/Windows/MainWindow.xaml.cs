using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;
using MarvinBrouwer.ServiceBusManager.Dialogs;
using MarvinBrouwer.ServiceBusManager.Services;

using Microsoft.Azure.Management.Fluent;
using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using TreeView = System.Windows.Controls.TreeView;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly IAzureServiceBusResourceCommandService _resourceCommandService;
	private readonly IAzureServiceBusResourceQueryService _resourceQueryService;
	private readonly IAzureAuthenticationService _azureAuthenticationService;
	private readonly IAzureLandscapeRenderingService _azureLandscapeRenderingService;
	private readonly ILocalStorageService _localStorageService;

	private bool _windowLoading;

	private static CancellationToken CancellationToken => ((App) Application.Current).CancellationToken;

	/// <inheritdoc />
	public MainWindow(
		IAzureAuthenticationService azureAuthenticationService,
		Func<TreeView, IAzureLandscapeRenderingService> azureLandscapeRenderingService,
		ILocalStorageService localStorageService,
		IAzureServiceBusResourceQueryService resourceQueryService,
		IAzureServiceBusResourceCommandService resourceCommandService)
	{
		InitializeComponent();

		_azureAuthenticationService = azureAuthenticationService;
		_azureLandscapeRenderingService = azureLandscapeRenderingService(AzureLandscape);
		_localStorageService = localStorageService;
		_resourceQueryService = resourceQueryService;
		_resourceCommandService = resourceCommandService;

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

	private async Task<IAzure> CreateAzureConnection(IAzureSubscription subscription)
	{
		AppendStatusMessage(@"Connecting to azure...");
		var credentials = await _azureAuthenticationService.Authenticate(subscription, CancellationToken);
		return credentials.WithSubscription(subscription.SubscriptionId);
	}

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

	private void SetupKeyBindings()
	{
		KeyDown += async (_, keyArgs) =>
		{
			if (keyArgs.Key == Key.F1) await ShowReadme();
			else if (keyArgs.Key == Key.Escape) HideReadme();

			if (keyArgs.Key == Key.F5 && !keyArgs.IsRepeat && !WindowLoading)
			{
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || AzureLandscape.SelectedItem is null)
					await ReloadAzureLandscape();
				else
				{
					await ReloadItem();
				}
			}
		};
	}
	#endregion

	private async void WindowLoaded()
	{
		WindowLoading = true;
		SetupKeyBindings();
		await _azureAuthenticationService.AuthenticateDefaultTenant(CancellationToken);
		_localStorageService.PrepareDownloadFolder();
		Cursor = Cursors.Arrow;
		await LoadFullAzureLandscape();
	}

	private async void ReloadAzureLandscape(object sender, RoutedEventArgs e)
	{
		await ReloadAzureLandscape();
	}

	private async Task ReloadAzureLandscape()
	{
		ClearStatusPanel();
		AppendStatusMessage(@"Reloading all data...");
		await LoadFullAzureLandscape();
	}

	private async Task LoadFullAzureLandscape()
	{
		if (AzureLandscape.SelectedItem is BaseTreeViewItem item) item.IsSelected = false;
		AzureLandscape.IsEnabled = false;
		foreach (AzureSubscriptionTreeViewItem treeViewItem in AzureLandscape.Items)
		{
			treeViewItem.IsEnabled = false;
			treeViewItem.IsExpanded = false;
		}

		WindowLoading = true;
		AzureLandscape.IsEnabled = true;

		var subscriptionTreeViewItems = _azureLandscapeRenderingService
			.LoadSubscriptions(CancellationToken);

		await subscriptionTreeViewItems.ToListAsync(CancellationToken);
		
		WindowLoading = false;
		AppendStatusMessage(@"Done!");
	}

	private bool WindowLoading
	{
		get => _windowLoading;
		set
		{
			_windowLoading = value;
			AzureLandscape.Cursor = value ? Cursors.AppStarting : Cursors.Arrow;
			ValidateButtonState();
		}
	}

	private async void ReloadItem(object sender, RoutedEventArgs e) => await ReloadItem();
	private async Task ReloadItem()
	{
		if (AzureLandscape.SelectedItem is null) return;
		if (AzureLandscape.SelectedItem is not AzureResourceTreeViewItem { CanReload: true } treeViewItem) return;

		ClearStatusPanel();

		var azure = await CreateAzureConnection(treeViewItem.AzureSubscription);
		AppendStatusMessage(@"Reloading item data...");
		WindowLoading = true;
		ValidateButtonState();

		if (AzureLandscape.SelectedItem is AzureSubscriptionTreeViewItem subscriptionTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadSubscriptionContents(azure, subscriptionTreeViewItem, CancellationToken);
		}

		if (AzureLandscape.SelectedItem is ServiceBusTreeViewItem serviceBusTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadServiceBusResources(azure, serviceBusTreeViewItem, CancellationToken);
		}

		if (AzureLandscape.SelectedItem is TopicTreeViewItem topicTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadTopicSubscriptions(azure, topicTreeViewItem, CancellationToken);
		}

		WindowLoading = false;
		AppendStatusMessage(@"Done!");
		ValidateButtonState();
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

	private async void ShowReadmeWindow(object sender, RoutedEventArgs e) => await ShowReadme();
	private async Task ShowReadme()
	{
		#if !DEBUG
		if (HelpFrame.Visibility == Visibility.Collapsed)
		#endif
		{
			ButtonReadme.IsEnabled = false;
			HelpFrame.Content = await HelpPage.Load();
			HelpFrame.Visibility = Visibility.Visible;
			ButtonReadme.IsEnabled = true;
		}

		HelpPanel.Visibility = Visibility.Visible;
	}

	private void HideReadmeWindow(object sender, RoutedEventArgs e) => HideReadme();
	private void HideReadme()
	{
		HelpPanel.Visibility = Visibility.Collapsed;
	}

	private static (int fullCount, bool maxItemsReached) ValidateItemCount(long fullCount)
	{
		if (fullCount <= AzureConstants.ServiceBusResourceMaxItemCount)
			return ((int)fullCount, false);

		return (AzureConstants.ServiceBusResourceMaxItemCount, true);
	}

	private async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReadAllResourceMessages(
		IAzure azure, ServiceBusResourceTreeViewItem treeViewItem, ServiceBusReceiveMode receiveMode)
	{
		AppendStatusMessage(
			receiveMode == ServiceBusReceiveMode.PeekLock
			? "Peeking message data"
			: "Reading message data (destructive)");

		var messages = await _resourceQueryService
			.ReadAllMessages(azure, treeViewItem.Resource, receiveMode, CancellationToken);

		AppendStatusMessage($"Downloaded {messages.Count} items");
		return messages;
	}
	private async Task StoreData(DateTime timestamp, IAzureResource selectedResource, IReadOnlyList<ServiceBusReceivedMessage> serviceBusMessages)
	{
		AppendStatusMessage(@"Storing resource data");

		await _localStorageService.StoreResourceDownload(timestamp, selectedResource, serviceBusMessages, CancellationToken);

		AppendStatusMessage(@"Download finished");
	}

	private async void ShowRequeueDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ServiceBusResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanRequeue) return;

		WindowLoading = true;
		ValidateButtonState();
		ClearStatusPanel();
		var azure = await CreateAzureConnection(treeViewItem.Resource.AzureSubscription);

		AppendStatusMessage("Getting item count to requeue");
		var fullCount = await _resourceQueryService.GetMessageCount(azure, treeViewItem.Resource, CancellationToken);
		var (itemCount, maxItemsReached) = ValidateItemCount(fullCount);

		var (requeue, storeDownload) = Dialog.ConfirmRequeue(treeViewItem, itemCount, maxItemsReached);
		if (!requeue)
		{
			AppendStatusMessage("Canceled");
			WindowLoading = false;
			ValidateButtonState();
			return;
		}

		var timestamp = DateTime.UtcNow;
		var serviceBusMessages = await ReadAllResourceMessages(azure, treeViewItem, ServiceBusReceiveMode.PeekLock);

		if (storeDownload) await StoreData(timestamp, treeViewItem.Resource, serviceBusMessages);
		
		await _resourceCommandService.QueueMessages(azure, treeViewItem.Resource, serviceBusMessages, CancellationToken);
		AppendStatusMessage($"Requeued {itemCount} items");
		WindowLoading = false;
		ValidateButtonState();
	}

	private async void ShowDownloadDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ServiceBusResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanDownload) return;

		WindowLoading = true;
		ValidateButtonState();
		ClearStatusPanel();
		var azure = await CreateAzureConnection(treeViewItem.Resource.AzureSubscription);

		AppendStatusMessage("Getting item count to download");
		var fullCount = await _resourceQueryService.GetMessageCount(azure, treeViewItem.Resource, CancellationToken);
		var (itemCount, maxItemsReached) = ValidateItemCount(fullCount);

		var download = Dialog.ConfirmDownload(treeViewItem, itemCount, maxItemsReached);
		if (!download)
		{
			AppendStatusMessage("Canceled");
			WindowLoading = false;
			ValidateButtonState();
			return;
		}

		var timestamp = DateTime.UtcNow;
		var serviceBusMessages = await ReadAllResourceMessages(azure, treeViewItem, ServiceBusReceiveMode.PeekLock);

		await StoreData(timestamp, treeViewItem.Resource, serviceBusMessages);

		AppendStatusMessage("Done!");
		WindowLoading = false;
		ValidateButtonState();
	}

	private async void ShowUploadDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ServiceBusResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanUpload) return;

		WindowLoading = true;
		ValidateButtonState();
		ClearStatusPanel();
		AppendStatusMessage("Selecting file(s) to upload");

		var openFileDialog = new OpenFileDialog
		{
			CheckFileExists = true,
			InitialDirectory = _localStorageService.DownloadFolderPath,
			Multiselect = true,
			Title = @"Select files to upload",
			ReadOnlyChecked = true,
			AddExtension = true,
			Filter = "Data|*.zip;*.json;*.xml;*.txt"
		};
		if (openFileDialog.ShowDialog() != true || openFileDialog.SafeFileNames.Length == 0)
		{
			AppendStatusMessage("No file selected");
			WindowLoading = false;
			ValidateButtonState();
			return;
		}

		AppendStatusMessage(openFileDialog.SafeFileNames.Length == 1
			? "1 file selected"
			: $"{openFileDialog.SafeFileNames.Length} files selected");
		AppendStatusMessage(openFileDialog.SafeFileNames.Length == 1
			? "Reading file..."
			: "Reading files");

		var dataToPush = await _localStorageService
			.ReadFileData(openFileDialog.FileNames, CancellationToken)
			.ToListAsync(CancellationToken);

		var upload = Dialog.ConfirmUpload(treeViewItem,
			string.Join(", ", openFileDialog.SafeFileNames), dataToPush.Count);
		if (!upload)
		{
			AppendStatusMessage("Canceled");
			WindowLoading = false;
			ValidateButtonState();
			return;
		}
		var azure = await CreateAzureConnection(treeViewItem.Resource.AzureSubscription);

		await _resourceCommandService
			.QueueMessages(azure, treeViewItem.Resource, dataToPush, CancellationToken);
		AppendStatusMessage($"Uploaded {dataToPush.Count} items");
		WindowLoading = false;
		ValidateButtonState();
	}

	private async void ShowClearDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not ServiceBusResourceTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanClear) return;

		WindowLoading = true;
		ValidateButtonState();
		ClearStatusPanel();
		var azure = await CreateAzureConnection(treeViewItem.Resource.AzureSubscription);

		AppendStatusMessage("Getting item count to clear");
		var fullCount = await _resourceQueryService.GetMessageCount(azure, treeViewItem.Resource, CancellationToken);
		var (itemCount, maxItemsReached) = ValidateItemCount(fullCount);

		var (clear, storeDownload) = Dialog.ConfirmClear(treeViewItem, itemCount, maxItemsReached);
		if (!clear)
		{
			AppendStatusMessage("Canceled");
			WindowLoading = false;
			ValidateButtonState();
			return;
		}

		var timestamp = DateTime.UtcNow;
		var serviceBusMessages = await ReadAllResourceMessages(azure, treeViewItem, ServiceBusReceiveMode.ReceiveAndDelete);

		if (storeDownload) await StoreData(timestamp, treeViewItem.Resource, serviceBusMessages);

		AppendStatusMessage($"Cleared {itemCount} items");
		WindowLoading = false;
		ValidateButtonState();
	}

	private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		ValidateButtonState();
	}

	private void ValidateButtonState()
	{
		if (AzureLandscape.SelectedItem is null)
			SelectedItemChanged(null);
		else if (AzureLandscape.SelectedItem is BaseTreeViewItem treeViewItem)
			SelectedItemChanged(treeViewItem);
		else SelectedItemChanged(null);
	}

	private void SelectedItemChanged(BaseTreeViewItem? treeViewItem)
	{
		ButtonClear.IsEnabled = treeViewItem is not null && !WindowLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanClear;
		ButtonDownload.IsEnabled = treeViewItem is not null && !WindowLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanDownload;
		ButtonRequeue.IsEnabled = treeViewItem is not null && !WindowLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanRequeue;
		ButtonUpload.IsEnabled = treeViewItem is not null && !WindowLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanUpload;
		
		MenuItemReload.IsEnabled = !WindowLoading;
		MenuItemReloadSelected.IsEnabled = treeViewItem is not null && !WindowLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanReload;

		CommandManager.InvalidateRequerySuggested();
	}
}