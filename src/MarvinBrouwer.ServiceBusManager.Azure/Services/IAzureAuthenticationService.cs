using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;

using VisualStudioCredential = Azure.Identity.VisualStudioCredential;
using IAuthenticated = Microsoft.Azure.Management.Fluent.Azure.IAuthenticated;
using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for authenticating the <see cref="ITenant"/>s to Azure
/// </summary>
/// <remarks>
/// Since this is a development tool, the default tenant will authenticate using the <see cref="VisualStudioCredential"/>
/// </remarks>
public interface IAzureAuthenticationService
{
	/// <summary>
	/// Authenticate the current system to list out the Azure <see cref="IAzureSubscription"/> and it's <see cref="ITenant"/>s
	/// </summary>
	/// <remarks>
	/// Since this is a development tool, the user will authenticate using the <see cref="VisualStudioCredential"/>
	/// </remarks>
	Task<IAuthenticated> AuthenticateDefaultTenant(CancellationToken cancellationToken);

	/// <summary>
	/// Generate a new <see cref="IAuthenticated"/> using the <see cref="ITenant"/> to create a new Token
	/// </summary>
	Task<IAuthenticated> Authenticate(ITenant tenant, CancellationToken cancellationToken);

	/// <summary>
	/// Generate a new <see cref="IAuthenticated"/> using the <see cref="ISubscription"/>'s default <see cref="ITenant"/> to create a new Token
	/// </summary>
	Task<IAuthenticated> Authenticate(IAzureSubscription subscription, CancellationToken cancellationToken);

}