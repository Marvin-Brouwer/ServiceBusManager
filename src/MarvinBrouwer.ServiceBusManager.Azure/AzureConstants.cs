namespace MarvinBrouwer.ServiceBusManager.Azure;

public static class AzureConstants
{
	/// <summary>
	/// Having more than 100 items appears to not be stable.
	/// This is only true for topics and queues, actual messages are batched by the api
	/// </summary>
	public const int ServiceBusResourceMaxItemCount = 100;

	/// <summary>
	/// Naming segment for dead letter items
	/// </summary>
	public const string DeadLetterPathSegment = "dead-letter";

	public const string DeadLetterSubQueueSegment = "$DeadLetterQueue";
}