# Introduction 
Contains helper methods for enabling and configuring Application Insights on Carnegie services and applications.

For more information on how to use Application Insights, see the [wiki page](https://dev.azure.com/carnegieinvestmentbank/CarnegieIT/_wiki/wikis/DefaultProject.wiki/221/Application-Insights).

## Project structure

* **Carnegie.ApplicationInsights.AspNetCore**
  * Helpers for enabling and configuring Application Insights for services and other Asp .Net Core projects.
* **Carnegie.ApplicationInsights.Common**
  * Shared code such as telemetry filters and processors.
  * Also contains `MonitoringHelper` that can be used for explicitly creating requests (useful for background services) and custom events.
* **Carnegie.ApplicationInsights.Logging**
  * A wrapper for the Serilog sink for Application Insights.
* **Carnegie.ApplicationInsights.MassTransit**
  * Sets up correlation ids for MassTransit producers and consumers.
* **Carnegie.ApplicationInsights.Worker**
  * Helpers for enabling and configuring Application Insights for non-web processes such as event-processing workers.