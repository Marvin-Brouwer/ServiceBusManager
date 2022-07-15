using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;

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

			// TODO figure out why this doesn't work
			Task.Factory.StartNew(
				(_) => Process.Start(Process.GetCurrentProcess().StartInfo),
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