using Carnegie.ApplicationInsights.MassTransit.Filters;
using Carnegie.ServiceBus.AspNetCore;

namespace Carnegie.ApplicationInsights.MassTransit
{
	public static class Extensions
	{
		/// <summary>
		/// Adding application insights filters to masstransit to pass and receive the operationId and parentId for this service
		/// NOTE: These filters require that a telemetry client is registered in the IoC container. If not, filters cannot be resolved
		/// and message delivery and receive will be disabled.
		/// </summary>
		/// <returns>ServiceBusOptions</returns>
		public static ServiceBusOptions AddApplicationInsights(this ServiceBusOptions serviceBusOptions)
		{
			serviceBusOptions.UseConsumeFilter(typeof(AppInsightsConsumeFilter<>));
			serviceBusOptions.UsePublishFilter(typeof(AppInsightsPublishFilter<>));

			return serviceBusOptions;
		}
	}
}
