using Microsoft.Azure.ServiceBus;

namespace MarvinBrouwer.ServiceBusManager.Azure.Helpers;

internal static class DeadLetterNameHelper
{

	/// <summary>
	/// Builds a format name from the specified dead letter queue path.
	/// </summary>
	/// <remarks>
	/// This is actually a copy from the <see>Microsoft.ServiceBus.Messaging.FormatDeadLetterPath</see>
	/// It didn't make sense to pull in an entire package for just one method
	/// </remarks>
	/// <param name="queuePath">The path to the dead letter queue.</param>
	/// <returns>The <see cref="T:System.String" /> resulted from building the format name for the specified dead letter queue path.</returns>
	public static string FormatDeadLetterPath(string queuePath) => EntityNameHelper.FormatSubQueuePath(queuePath, AzureConstants.DeadLetterSubQueueSegment);

}
