using System;
using System.Windows.Controls;
using MarvinBrouwer.ServiceBusManager.Azure;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MarvinBrouwer.ServiceBusManager;

internal static class Startup
{
	public static void ConfigureServices(IServiceCollection services)
	{
		services.AddScoped(ConfigureLandscapeServiceFactory);
		services.AddScoped<LocalStorageService>();

		services.ConfigureServiceBusManagerAzureServices();
	}

	private static Func<TreeView, AzureLandscapeRenderingService> ConfigureLandscapeServiceFactory(IServiceProvider services)
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
