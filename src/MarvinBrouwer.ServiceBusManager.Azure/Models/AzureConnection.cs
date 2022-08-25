using Microsoft.Azure.Management.ResourceManager.Fluent;
using IAuthenticated = Microsoft.Azure.Management.Fluent.Azure.IAuthenticated;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Wrapper class for a full authenticated connection to azure.
/// </summary>
public class AzureConnection
{
	private DateTimeOffset ExpiresOn { get; }

	/// <inheritdoc cref="AzureConnection"/>
	public AzureConnection(ITenant tenant, AzureAuthentication authentication)
	{
		Tenant = tenant;

		AzureClient = authentication.AzureClient;
		ExpiresOn = authentication.AuthenticatedToken.ExpiresOn;
	}

	/// <summary>
	/// The <see cref="ITenant"/>(azure account) associated to this <see cref="AzureClient"/>
	/// </summary>
	public ITenant Tenant { get; }

	/// <inheritdoc cref="AzureAuthentication.AzureClient"/>
	public IAuthenticated AzureClient { get; }

	/// <summary>
	/// Describes whether this connection is still authenticated or not.
	/// </summary>
	/// <remarks>
	/// Expires 2 min before the actual token expiration.
	/// </remarks>
	public bool Expired => ExpiresOn <= DateTimeOffset.UtcNow.AddMinutes(2);
}
