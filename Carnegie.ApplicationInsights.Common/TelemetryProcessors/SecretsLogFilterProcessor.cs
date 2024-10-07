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
        private const string _carnegieJWT = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2Iiwia2lkIjoi";
        private const string _regexClientSecret = "(?<=client_secret=)[0-9a-zA-Z]*";
        private const string _regexSSN = "(?<=subject-)\\d{12}";
        private const string _regexAccessToken = "(?<=ey)[\\w-]+\\.[\\w-]+\\.[\\w-]+";
        private const string _regexSubject = "(?<=subject=)[0-9a-zA-Z]*";

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
                
                if (ContainsJWT(dependency.Data))
                    dependency.Data = Regex.Replace(dependency.Data, _regexAccessToken, "***");

                if (dependency.Data.Contains("client_secret="))
                    dependency.Data = Regex.Replace(dependency.Data, _regexClientSecret, "***");                   
                
                if (dependency.Data.Contains("login_hint=subject-"))
                    dependency.Data = Regex.Replace(dependency.Data, _regexSSN, "***");

                if (dependency.Data.Contains("?subject="))
                    dependency.Data = Regex.Replace(dependency.Data, _regexSubject, "***");
            }
            else if (item is RequestTelemetry request)
            {
                if (request.Url == null)
                    return;

                string requestUrl = request.Url.ToString();

                if (ContainsJWT(requestUrl))
                    request.Url = new Uri(Regex.Replace(requestUrl, _regexAccessToken, "***"));

                if(ContainsSubjectParameter(requestUrl))
                    request.Url = new Uri(Regex.Replace(requestUrl, _regexSubject, "***"));
            }
        }

        private bool ContainsSubjectParameter(string input)
        {
            return input.Contains("?subject=") || input.Contains("&subject=");
        }

        private bool ContainsJWT(string input)
        {
            return input.Contains("Bearer") || input.Contains("access_token=") || input.Contains(_carnegieJWT);
        }
    }
}
