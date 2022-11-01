using Azure.Core;

using IAuthenticated = Microsoft.Azure.Management.Fluent.Azure.IAuthenticated;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Wrapper class for an authenticated azure client, with assigned token data
/// </summary>
public class AzureAuthentication
{
	/// <summary>
	/// The <see cref="IAuthenticated"/> Fluent.Azure instance to interact with the Azure API
	/// </summary>
	public IAuthenticated AzureClient { get; init; } = default!;

	/// <summary>
	/// The <see cref="AccessToken"/> associated to the <see cref="AzureClient"/> <br />
	/// By default there's not way to get back the token from an <see cref="IAuthenticated"/>
	/// so we store it ourselves.
	/// </summary>
	public AccessToken AuthenticatedToken { get; init; } = default!;
}
