using MarvinBrouwer.ServiceBusManager.Windows;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MarvinBrouwer.ServiceBusManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private readonly CancellationTokenSource _applicationCancellationTokenSource = new();
	public CancellationToken CancellationToken => _applicationCancellationTokenSource.Token;

	private void App_OnStartup(object sender, StartupEventArgs e)
	{
		Exit += (_, _) => { SignalCancel(); };
		AppDomain.CurrentDomain.ProcessExit += (_, _) => { SignalCancel(); };
		Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;

		var serviceContainer = new ServiceCollection();
		ServiceBusManager.Startup.ConfigureServices(serviceContainer);

		serviceContainer.AddScoped<MainWindow>();
		MainWindow = serviceContainer
			.BuildServiceProvider()
			.GetRequiredService<MainWindow>();

		ShutdownMode = ShutdownMode.OnMainWindowClose;
		MainWindow.Show();
	}

	private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		var exception = e.Exception;

		if (!Debugger.IsAttached && exception is not OperationCanceledException)
		{
			MessageBox.Show(
				$@"{exception.Message}{Environment.NewLine}{Environment.NewLine}{exception.StackTrace ?? string.Empty}",
				@$"Unhandled exception: {exception.GetType().FullName}",
				MessageBoxButton.OK, MessageBoxImage.Error);

			// Don't restart when error on close.
			if (CancellationToken.IsCancellationRequested) return;

			var currentProcess = Process.GetCurrentProcess();
			var restartProcessInfo = new ProcessStartInfo
			{
				FileName = currentProcess.MainModule!.FileName,
				WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
			};
			
			Task.Factory.StartNew(
				(_) =>
				{
					Process.Start(restartProcessInfo);
					Shutdown();
				},
				CancellationToken.None,
				TaskCreationOptions.RunContinuationsAsynchronously);

		}

		SignalCancel();
	}

	public void SignalCancel()
	{
		_applicationCancellationTokenSource.Cancel();
	}
}