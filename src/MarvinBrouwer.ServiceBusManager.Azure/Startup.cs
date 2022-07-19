using MarvinBrouwer.ServiceBusManager.Azure.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MarvinBrouwer.ServiceBusManager.Azure;

/// <summary>
/// Startup extension class
/// </summary>
public static class Startup
{
	/// <summary>
	/// Configure all the services necessary to get access to the Azure resources we need
	/// </summary>
	public static void ConfigureServiceBusManagerAzureServices(this IServiceCollection services)
	{
		services.AddScoped<IAzureAuthenticationService, AzureAuthenticationService>();
		services.AddScoped<IAzureSubscriptionService, AzureSubscriptionService>();
		services.AddScoped<IAzureServiceBusService, AzureServiceBusService>();
		services.AddScoped<IAzureServiceBusResourceQueryService, AzureServiceBusResourceQueryService>();
		services.AddScoped<IAzureServiceBusResourceCommandService, AzureServiceBusResourceCommandService>();
	}
}
