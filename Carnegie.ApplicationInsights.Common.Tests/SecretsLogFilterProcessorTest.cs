using Carnegie.ApplicationInsights.Common.TelemetryProcessors;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using System;
using System.Text.RegularExpressions;
using Xunit;

namespace CarnegieApplicationInsightsCommonTests 
{
    public class SecretsLogFilterProcessorTest
    {
        const string _bearerTestURL = "https://mytest.se/index.html?access_token=eyJhbGciOiJodHRwOi8ck3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2Iiwia2lkIjoiNDEyODlDMUU0NTg0MUU0NjkwMzNCQTNFRjBGQzkzMEIyRTcwNTg2OCIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1ZjUwYWEwZC1hOGFkLTQ4ZjYtODg0Yi00NTg5ZjFhZTRmZGIiLCJ0eXAiOiJKV1QiLCJzdWIiOiI5YzVhMWVjYS1iN2Y5LTQ4ZGMtOGYyZS04ZDMwZGMyZWQ4ZDUiLCJhcHBpZCI6IkZpUHJvIiwidW5pcXVlX25hbWUiOiJ6YWluYWIuYW1lci1hZ3JlbkBjYXJuZWdpZS5zZSIsImZpcnN0bmFtZSI6IlphaW5hYiIsImxhc3RuYW1lIjoiQW1lci3DhWdyZW4iLCJkaXNwbGF5bmFtZSI6IlphaW5hYiBBbWVyLcOFZ3JlbiIsImVtYWlsIjoiemFpbmFiLmFtZXItYWdyZW5AY2FybmVnaWUuc2UiLCJkZXBhcnRtZW50IjoiU0UgVHJlYXN1cnkgJiBDcmVkaXQiLCJidXNpbmVzc19hcmVhIjoiYzZjYzBhYjEtMzk1MS00ZmZjLWE3ZDctZWQzNzg2ZGYyM2UzIiwiY291bnRyeSI6IlN3ZWRlbiIsIndtVXNlcklkIjoiNzU5Iiwicm9sZXMiOlsiZW1wbG95ZWUiLCJmaXByby1hZG1pbiJdLCJuYmYiOjE2NDc5MzQ3NTEsImV4cCI6MTY0Nzk3Nzk1MSwiaXNzIjoiYXV0aC5jYXJuZWdpZS5zZSJ9.A6EiPQMvGxFRqOve5gex3Mw7hh6wXCQ2Z9UpOGy-C_pSFLvV_lLsbOSCIE9FTJO-sLf33CIiMCLen2d4e9wIts571w_YA8qVUcbJ2adn76BMhoLbImI9uzUSXIh3ogmAzBgD16AtlUEJQGSnkZyxeTzTydVs6ZyRq8HG_Ar0k1V_xJN8Sgqv26PaN0JJwvyMZECXl_6lLWzMREOSCSpY9v2ZxN9sgrSTMjN-mK99cvT_KbKO0VGi5hM7QTaTkuaM0LYfiVEW8Qs1mlZ5aIrz7egXAYycuoQqctT264ReCqEqi1wgjyAOaIeOXNfgUlW9z224GEV0cYH7M_hvT_WtIA&token_type=Bearer&expires_in=43200";
        const string _ssnTestURL = "https://mytest.se/oidc/authorize?response_type=code&scope=openid+profile+signicat.national_id&client_id=prod.carnegie.se&client_secret=b5YNKXFhZq9PP3ZRnUReE1wh09GK4nYSkerGBewRQb4&redirect_uri=https://login-mytest.se&acr_values=urn:signicat:oidc:method:sbid-inapp-oidc&state=0.8984038630102927&login_hint=subject-194810335453";
        const string _internalTokenTestURL = "https://mytest-service.se/advisory?id=J00-pxmBkk-NjYYeX-SMQg&access_token=eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2Iiwia2lkIjoiNDEyODlDMUU0NTg0MUU0NjkwMzNCQTNFRjBGQzkzMEIyRTcwNTg2OCIsInR5cCI6IkpXVCJ9.eyJqdGkiOOllMGVkNTgzMi02OGUxLTRmOTktOGM0ZS01NjliMDU1Nzk3ZDAiLCJ0eXAiOiJKV1QiLCJzdWIiOiIyNDA3NTE1Ni1iMmEyLTQxMDEtYWNiZi00ZWRlN2ZkOTIxZGQiLCJhcHBpZCI6IlBhbmRhLXByb2QiLCJ1bmlxdWVfbmFtZSI6ImFuZHJlYXMuZmxlbW1lckBjYXJuZWdpZS5zZSIsImZpcnN0bmFtZSI6IkFuZHJlYXMiLCJsYXN0bmFtZSI6IkZsZW1tZXIiLCJkaXNwbGF5bmFtZSI6IkFuZHJlYXMgRmxlbW1lciIsImVtYWlsIjoiYW5kcmVhcy5mbGVtbWVyQGNhcm5lZ2llLnNlIiwiZGVwYXJ0bWVudCI6IlNFIFBCIEFzc2V0IE1nbW50IiwiYnVzaW5lc3NfYXJlYSI6ImQxNmExZDYyLWQ4YzUtNDcxOS05OGM4LTU1MmQ5NGYxYTE5MiIsImNvdW50cnkiOiJTd2VkZW4iLCJ3bVVzZXJJZCI6IjEzNDciLCJyb2xlcyI6ImVtcGxveWVlIiwibmJmIjoxNjQ4NDU2NDkwLCJleHAiOjE3IDg0OTk2OTAsImlzcyI6ImF1dGguY2FybmVnaWUuc2UifQ.oX8M1rGvCzyb0lcfMDS27rOblMiJ-X6R7N7LvABqiAxlVcK8LN3tYQCFcrhkSOw9Nq_lNFspwB3lIkWQlZFXjEAfcrFn6p8dAkzkPEqhy3_bm0Dav985XdsGuF9Vp1c3cwjcHTK7t-D4XKPJvrn6cXQXK6ypi4FPRRXey1Y33g4OMjXXOxkMeL1-EoE_JHW_yaQmSg0qvfQJ8i9zkoYwhI6CSmXv-8DSmIWu7S7n594rBHID7Af7KabiPt6MSovll07UfmyEN4Vx5Bm90vMQ2AtgOYJvarrmaXGPJukjfthyxD6KYpUXNI-iJzP5bnRDMXCivxz3ave0QGp1re87Fw";
        const string _carnegieJWT = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2Iiwia2lkIjoiNDEyODlDMUU0NTg0MUU0NjkwMzNCQTNFRjBGQzkzMEIyRTcwNTg2OCIsInR5cCI6IkpXVCJ9.";

        private readonly string _regexAccessToken = "(?<=ey)[\\w-]+\\.[\\w-]+\\.[\\w-]+";
        private readonly string _regexClientSecret = "(?<=client_secret=)[0-9a-zA-Z]*";
        private readonly string _regexSSN = "(?<=subject-)\\d{12}";

        [Fact]
        public void DependencyTelemetry_exception_should_be_null()
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var dependency = new DependencyTelemetry();
            dependency.Data = null;

            Exception exception = Record.Exception(() => { sut.Process(dependency); });
            Assert.True(exception == null);
        }

        [Theory]
        [InlineData(_bearerTestURL, "https://mytest.se/index.html?access_token=ey***&token_type=Bearer&expires_in=43200")]
        [InlineData("https://mytest.se/index.html?access_token=eyBlaBla.8142Bla.bla814BLA&token_type=Bearer&expires_in=43200", "https://mytest.se/index.html?access_token=ey***&token_type=Bearer&expires_in=43200")]
        public void RequestTelemetry_bearer_should_be_equal(string input, string expectedOutput)
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var request = new RequestTelemetry();
            request.Url = new Uri(input);

            sut.Process(request);
            Assert.Equal(expectedOutput, request.Url.ToString());
        }

        [Theory]
        [InlineData(_internalTokenTestURL, "https://mytest-service.se/advisory?id=J00-pxmBkk-NjYYeX-SMQg&access_token=ey***")]
        [InlineData("https://mytest-service.se/test?access_token=" + _carnegieJWT + "eyBlat3St.T3sTinG", "https://mytest-service.se/test?access_token=ey***")]
        public void RequestTelemetry_internal_token_should_be_equal(string url, string expectedOutput)
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var request = new RequestTelemetry();
            request.Url = new Uri(url);

            sut.Process(request);
            Assert.Equal(expectedOutput, request.Url.ToString());
        }

        [Theory]
        [InlineData(_bearerTestURL, "https://mytest.se/index.html?access_token=ey***&token_type=Bearer&expires_in=43200")]
        [InlineData("https://mytest.se/index.html?access_token=eyBlaBla.8142Bla.bla814BLA&token_type=Bearer&expires_in=43200", "https://mytest.se/index.html?access_token=ey***&token_type=Bearer&expires_in=43200")]
        [InlineData("Bearer eyTestr3fs.t3fs4st.BLaerfw814", "Bearer ey***")]
        [InlineData("Bearer eyJhbGciOiJIUzI1NiJ9.eyJuYW1lIjoiSm9lIENvZGVyIn0.5dlp7GmziL2QS06sZgK4mtaqv0xX4oFUuTDh1zHK4U", "Bearer ey***")]
        public void DependencyTelemetry_bearer_should_be_equal(string input, string expectedOutput)
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var dependency = new DependencyTelemetry();
            dependency.Data = input;

            sut.Process(dependency);
            Assert.Equal(expectedOutput, dependency.Data);
        }

        [Theory]
        [InlineData("eyJhbGciOiJIUzI1NiJ9.eyJuYW1lIjoiSm9lIENvZGVyIn05dlp7GmziL2QS06sZgK4mtaqv0xX4oFUuTDh1zHK4U", "ey***")]
        [InlineData("eyJigojsjsf.OPJfopjafkpaf.JGldjfsf&", "ey***")]
        public void DependencyTelemetry_bearer_should_not_be_equal(string input, string expectedOutput)
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var dependency = new DependencyTelemetry();
            dependency.Data = input;

            sut.Process(dependency);
            Assert.NotEqual(expectedOutput, dependency.Data);
        }

        [Theory]
        [InlineData(_internalTokenTestURL, "https://mytest-service.se/advisory?id=J00-pxmBkk-NjYYeX-SMQg&access_token=ey***")]
        [InlineData("https://mytest-service.se/test?access_token=" + _carnegieJWT + "eyBlat3St.T3sTinG", "https://mytest-service.se/test?access_token=ey***")]
        [InlineData("Bearer eyTestr3fs.t3fs4st.BLaerfw814", "Bearer ey***")]
        [InlineData("Bearer eyJhbGciOiJIUzI1NiJ9.eyJuYW1lIjoiSm9lIENvZGVyIn0.5dlp7GmziL2QS06sZgK4mtaqv0xX4oFUuTDh1zHK4U", "Bearer ey***")]
        public void DependencyTelemetry_internal_token_should_be_equal(string url, string expectedOutput)
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var dependency = new DependencyTelemetry();
            dependency.Data = url;

            sut.Process(dependency);
            Assert.Equal(expectedOutput, dependency.Data);
        }

        [Theory]
        [InlineData(_ssnTestURL, "https://mytest.se/oidc/authorize?response_type=code&scope=openid+profile+signicat.national_id&client_id=prod.carnegie.se&client_secret=***&redirect_uri=https://login-mytest.se&acr_values=urn:signicat:oidc:method:sbid-inapp-oidc&state=0.8984038630102927&login_hint=subject-***")]
        [InlineData("login_hint=subject-194205309785", "login_hint=subject-***")]
        [InlineData("client_secret=testbl4t3st", "client_secret=***")]
        public void DependencyTelemetry_client_secret_and_SSN_should_be_equal(string text, string expectedOutput)
        {
            var telemetryProcessor = Mock.Of<ITelemetryProcessor>();
            var sut = new SecretsLogFilterProcessor(telemetryProcessor);

            var dependency = new DependencyTelemetry();
            dependency.Data = text;

            sut.Process(dependency);
            Assert.Equal(expectedOutput, dependency.Data);
        }

        [Theory]
        [InlineData(_bearerTestURL)]
        [InlineData(_internalTokenTestURL)]
        [InlineData("eyteST34TEst.t3st.T3ST1nG")]
        public void Token_should_be_found(string text)
        {
            var testMatch = Regex.IsMatch(text, _regexAccessToken);

            Assert.True(testMatch);
        }

        [Theory]
        [InlineData("eyteST34TEst.t3st.")]
        [InlineData("eyteST34TEstt3stT3ST1nG")]
        [InlineData("eyteST34.TEstt3stT3ST1nG")]
        [InlineData("teST34TEst.t3st.T3ST1nG")]
        public void Token_should_not_be_found(string text)
        {
            var testMatch = Regex.IsMatch(text, _regexAccessToken);

            Assert.False(testMatch);
        }

        [Theory]
        [InlineData(_ssnTestURL)]
        [InlineData("Testclient_secret=43jedsdpjf38sf0sda8")]
        [InlineData("Testclient_secret=43jedsdpjf38sf0sda8&blabla")]
        public void Client_secret_should_be_found(string text)
        {
            var testMatch = Regex.IsMatch(text, _regexClientSecret);

            Assert.True(testMatch);
        }

        [Theory]
        [InlineData(_ssnTestURL)]
        [InlineData("subject-194290478733")]
        [InlineData("Testsubject-194290478733Test")]
        public void SSN_should_be_found(string text)
        {
            var testMatch = Regex.IsMatch(text, _regexSSN);

            Assert.True(testMatch);
        }

        [Theory]
        [InlineData("subject-4290478733")]
        [InlineData("194290478733")]
        public void SSN_should_not_be_found(string text)
        {
            var testMatch = Regex.IsMatch(text, _regexSSN);

            Assert.False(testMatch);
        }

        [Theory]
        [InlineData("client_secret=testhemlighet89234fsfs8xz8s")]
        [InlineData("dsgfclient_secret=testhemlighet89234fsfs8xz8s?dsf")]
        [InlineData(".client_secret=testhemlighet89234fsfs8xz8s.")]
        [InlineData("blabla&client_secret=testhemlighet89234fsfs8xz8s&blabla")]
        public void Client_secret_should_match(string text)
        {
            string expectedOutput = "testhemlighet89234fsfs8xz8s";
            var testMatch = Regex.Match(text, _regexClientSecret);

            Assert.Equal(expectedOutput, testMatch.Value);
        }

        [Theory]
        [InlineData(_ssnTestURL)]
        [InlineData("subject-194810335453")]
        [InlineData("blablablasubject-194810335453")]
        [InlineData("blablablasubject-19481033545343535")]
        [InlineData("blablablasubject-194810335453test")]
        public void SSN_should_match(string text)
        {
            string expectedOutput = "194810335453";
            var testMatch = Regex.Match(text, _regexSSN);
            
            Assert.Equal(expectedOutput, testMatch.Value);
        }
    }
}