using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Internals;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Carnegie.ApplicationInsights.MassTransit.Filters
{
    public class AppInsightsPublishFilter<T> : IFilter<PublishContext<T>>, IFilter<SendContext<T>>
        where T : class
    {
        private readonly TelemetryClient _telemetryClient;


        public AppInsightsPublishFilter(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        async Task IFilter<PublishContext<T>>.Send(PublishContext<T> context, IPipe<PublishContext<T>> next) => await Send(context, next);

        async Task IFilter<SendContext<T>>.Send(SendContext<T> context, IPipe<SendContext<T>> next) => await Send(context, next);

        private async Task Send<TSendContext>(TSendContext context, IPipe<TSendContext> next) where TSendContext : class, SendContext<T>
        {
            var operation = _telemetryClient.StartOperation<DependencyTelemetry>(typeof(T).FullName);

            context.Headers.Set(ApplicationInsightsConstants.RootId, operation.Telemetry.Context.Operation.Id);
            context.Headers.Set(ApplicationInsightsConstants.ParentId, operation.Telemetry.Id);
            context.Headers.Set(ApplicationInsightsConstants.Parent, _telemetryClient.Context.Cloud.RoleName);

            operation.Telemetry.Type = ApplicationInsightsConstants.TelemetryType;

            var type = typeof(TSendContext).HasInterface(typeof(PublishContext)) ? "PUBLISH" : "SEND";

            operation.Telemetry.Data = $"{type} {context.DestinationAddress}";

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