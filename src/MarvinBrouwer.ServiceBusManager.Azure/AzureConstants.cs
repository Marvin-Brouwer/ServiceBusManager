namespace MarvinBrouwer.ServiceBusManager.Azure;

public static class AzureConstants
{
	/// <summary>
	/// This is not allowed to be more than 100, so if we need more we'll need to batch
	/// This is only true for topics and queues, actual messages are batched by the api
	/// </summary>
	public const int ServiceBusResourceMaxItemCount = 100;


	/// <summary>
	/// This is not allowed to be more than 100, so if we need more we'll need to batch
	/// This is only true for topics and queues, actual messages are batched by the api
	/// </summary>
	public const int GetMessageCount = 25;

	/// <summary>
	/// Naming segment for dead letter items
	/// </summary>
	public const string DeadLetterPathSegment = "dead-letter";

	public const string DeadLetterSubQueueSegment = "$DeadLetterQueue";
}