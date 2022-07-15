using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Services;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows.Controls;

namespace MarvinBrouwer.ServiceBusManager;

internal static class Startup
{
	public static void ConfigureServices(IServiceCollection services)
	{
		services.AddScoped(ConfigureLandscapeServiceFactory);
		services.AddScoped<ILocalStorageService, LocalStorageService>();

		services.ConfigureServiceBusManagerAzureServices();
	}

	private static Func<TreeView, IAzureLandscapeRenderingService> ConfigureLandscapeServiceFactory(IServiceProvider services)
	{
		return (treeView) =>
		{
			var azureSubscriptionService = services.GetRequiredService<IAzureSubscriptionService>();
			var azureServiceBusService = services.GetRequiredService<IAzureServiceBusService>();

			return new AzureLandscapeRenderingService(treeView,
				azureSubscriptionService,
				azureServiceBusService);
		};
	}
}
