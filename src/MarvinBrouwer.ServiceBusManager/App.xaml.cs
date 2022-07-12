using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MarvinBrouwer.ServiceBusManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private CancellationTokenSource _applicationCancellationTokenSource = new();
	public CancellationToken CancellationToken => _applicationCancellationTokenSource.Token;

	private void App_OnStartup(object sender, StartupEventArgs e)
	{
		Exit += (_, _) => { SignalCancel(); };
		AppDomain.CurrentDomain.ProcessExit += (_, _) => { SignalCancel(); };

		Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;
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
			Process.Start(Process.GetCurrentProcess().StartInfo);
		}

		SignalCancel();
	}

	public void SignalCancel()
	{
		_applicationCancellationTokenSource.Cancel();
	}
}