using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Carnegie.ApplicationInsights.Common.TelemetryProcessors
{
    public class SecretsLogFilterProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        private const string _carnegieJWT = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2Iiwia2lkIjoiNDEyODlDMUU0NTg0MUU0NjkwMzNCQTNFRjBGQzkzMEIyRTcwNTg2OCIsInR5cCI6IkpXVCJ9.";
        private const string __regexClientSecret = "(?<=client_secret=)[0-9a-zA-Z]*";
        private const string __regexSSN = "(?<=subject-)\\d{12}";
        private const string __regexAccessToken = "(?<=ey)[\\w-]+\\.[\\w-]+\\.[\\w-]+";

        public SecretsLogFilterProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            FilterBearerTokenClientSecretsAndSSN(item);

            // Send the item to the next TelemetryProcessor
            _next.Process(item);
        }

        private void FilterBearerTokenClientSecretsAndSSN(ITelemetry item)
        {
            if (item is DependencyTelemetry dependency)
            {
                if (dependency.Data == null)
                    return;
                
                if (dependency.Data.Contains("Bearer") || dependency.Data.Contains(_carnegieJWT))
                    dependency.Data = Regex.Replace(dependency.Data, __regexAccessToken, "***");

                if (dependency.Data.Contains("client_secret="))
                    dependency.Data = Regex.Replace(dependency.Data, __regexClientSecret, "***");                   
                
                if (dependency.Data.Contains("login_hint=subject-"))
                    dependency.Data = Regex.Replace(dependency.Data, __regexSSN, "***");
            }
            else if (item is RequestTelemetry request)
            {
                if (request.Url == null)
                    return;

                string requestUrl = request.Url.ToString();

                if (requestUrl.Contains("Bearer") || requestUrl.Contains(_carnegieJWT))
                    request.Url = new Uri(Regex.Replace(requestUrl, __regexAccessToken, "***"));
            }
        }
    }
}
