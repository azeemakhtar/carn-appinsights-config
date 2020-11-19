using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Carnegie.ApplicationInsights.MassTransit.Filters
{
	public class AppInsightsPublishFilter<T> : IFilter<PublishContext<T>>
		where T : class
	{
		private readonly TelemetryClient _telemetryClient;


		public AppInsightsPublishFilter(TelemetryClient telemetryClient)
		{
			_telemetryClient = telemetryClient;
		}

		public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
		{
			var operation = _telemetryClient.StartOperation<DependencyTelemetry>(typeof(T).FullName);
			
            context.Headers.Set(ApplicationInsightsConstants.RootId, operation.Telemetry.Context.Operation.Id);
			context.Headers.Set(ApplicationInsightsConstants.ParentId, operation.Telemetry.Id);
			context.Headers.Set(ApplicationInsightsConstants.Parent, _telemetryClient.Context.Cloud.RoleName);

			operation.Telemetry.Type = ApplicationInsightsConstants.TelemetryType;
			operation.Telemetry.Data = "PUBLISH " + context.DestinationAddress;
			
            Console.WriteLine($"Publish message at {DateTime.Now}");
			try
			{
				await next.Send(context);
				operation.Telemetry.Success = true;
			}
			catch (Exception)
			{
				operation.Telemetry.Success = false;
				throw;
			}
			finally
			{
				_telemetryClient.StopOperation(operation);
			}
		}

		public void Probe(ProbeContext context)
		{
		}
	}
}