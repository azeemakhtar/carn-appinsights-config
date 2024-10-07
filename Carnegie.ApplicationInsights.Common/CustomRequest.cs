using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carnegie.ApplicationInsights.Common
{
    public class CustomRequest : IDisposable
    {
        private readonly IOperationHolder<RequestTelemetry> _request;

        public CustomRequest(IOperationHolder<RequestTelemetry> request)
        {
            _request = request;
        }

        public void Dispose()
        {
            _request?.Dispose();
        }
    }
}