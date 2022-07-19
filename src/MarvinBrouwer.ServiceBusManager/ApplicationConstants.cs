using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager;

internal static class ApplicationConstants
{
	/// <summary>
	/// Naming segment for dead letter items
	/// </summary>
	public const string DeadLetterPathSegment = "dead-letter";
}
