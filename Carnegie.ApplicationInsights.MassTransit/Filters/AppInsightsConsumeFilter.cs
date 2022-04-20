using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Carnegie.ApplicationInsights.MassTransit.Filters
{
	public class AppInsightsConsumeFilter<T> : IFilter<ConsumeContext<T>>
		where T : class
	{
		private readonly TelemetryClient _telemetryClient;

		public AppInsightsConsumeFilter(TelemetryClient telemetryClient)
		{
			_telemetryClient = telemetryClient;
		}

		public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
		{
            var operationName = typeof(T).FullName;
            var requestTelemetry = new RequestTelemetry { Name = operationName };
			requestTelemetry.Context.Operation.Id = context.Headers.Get<string>(ApplicationInsightsConstants.RootId);
			requestTelemetry.Context.Operation.ParentId = context.Headers.Get<string>(ApplicationInsightsConstants.ParentId);
			requestTelemetry.Context.Operation.Name = operationName;
            

			if(context.SentTime.HasValue)
            {
                var duration = DateTime.UtcNow - context.SentTime.Value;
                _telemetryClient.TrackRequest(operationName, context.SentTime.Value, duration, "Delivered", true);
            }

            using var operation = _telemetryClient.StartOperation(requestTelemetry);

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