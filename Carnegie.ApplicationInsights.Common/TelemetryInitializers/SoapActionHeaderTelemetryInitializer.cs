using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carnegie.ApplicationInsights.Common.TelemetryInitializers
{
    /// <summary>
    /// Appends the SOAP Action to logged WCF dependencies.
    ///
    /// Custom properties added:
    /// - SOAPAction - "http://services.tieto.com/abasec/basicdata/Accounts/IAccounts/GetAccounts"
    /// - SOAPActionShort - "GetAccounts"
    /// </summary>
    /// <remarks>
    /// Source: https://stackoverflow.com/a/66858510/736684
    /// </remarks>
    public class SoapActionHeaderTelemetryInitializer : ITelemetryInitializer
    {
        private static readonly Regex SoapActionUri = new Regex("^\"(?<uri>.*)\"$", RegexOptions.Compiled);

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry httpDependency)
            {
                httpDependency.Context.TryGetRawObject("HttpRequest", out var request);
                if (request is HttpRequestMessage httpRequest)
                {
                    if (httpRequest.Headers.TryGetValues("SOAPAction", out var values))
                    {
                        var soapActionHeader = values.FirstOrDefault();
                        if (soapActionHeader != null)
                        {
                            // SOAP Action is contained within quote : https://www.w3.org/TR/2000/NOTE-SOAP-20000508/#_Toc478383528 
                            var soapAction = SoapActionUri.Match(soapActionHeader).Groups["uri"].Value;

                            telemetry.Context.GlobalProperties["SOAPAction"] = soapAction;
                            telemetry.Context.GlobalProperties["SOAPActionShort"] = soapAction.Split('/').LastOrDefault();
                        }
                    }
                }
            }
        }
    }
}