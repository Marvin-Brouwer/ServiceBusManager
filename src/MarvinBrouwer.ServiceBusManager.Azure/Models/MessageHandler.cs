using Azure.Messaging.ServiceBus;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record MessageHandler(
	ServiceBusClient Client,
	ServiceBusResource Resource,
	ServiceBusReceiver Receiver, 
	IReadOnlyList<ServiceBusReceivedMessage> Messages
) : IAsyncDisposable
{
	private bool _disposedValue;

	public async ValueTask DisposeAsync()
	{
		if (_disposedValue) return;

		await Client.DisposeAsync();

		if (!Receiver.IsClosed) await Receiver.CloseAsync();
		await Receiver.DisposeAsync();

		_disposedValue = true;
	}
}