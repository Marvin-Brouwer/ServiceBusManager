using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MarvinBrouwer.ServiceBusManager.Azure;

public static class Startup
{
	public static void ConfigureServiceBusManagerAzureServices(this IServiceCollection services)
	{
		services.AddScoped<IAzureAuthenticationService, AzureAuthenticationService>();
		services.AddScoped<IAzureSubscriptionService, AzureSubscriptionService>();
		services.AddScoped<IAzureServiceBusService, AzureServiceBusService>();
		services.AddScoped<IAzureServiceBusResourceQueryService, AzureServiceBusResourceQueryService>();
	}
}
