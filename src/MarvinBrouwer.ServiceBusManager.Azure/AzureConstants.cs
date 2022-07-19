namespace MarvinBrouwer.ServiceBusManager.Azure;

/// <summary>
/// Constant values used for working with Azure Service Buses
/// </summary>
public static class AzureConstants
{
	/// <summary>
	/// Having more than 100 items appears to not be stable.
	/// This is only true for topics and queues, actual messages are batched by the api
	/// </summary>
	public const int ServiceBusResourceMaxItemCount = 100;

	/// <summary>
	/// The path segment Azure adds to any dead-letter queue/topicSubscription
	/// </summary>
	public const string DeadLetterSubQueueSegment = "$DeadLetterQueue";
}