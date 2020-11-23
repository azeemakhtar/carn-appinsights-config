﻿using Carnegie.ApplicationInsights.Common.TelemetryInitializers;
using Carnegie.ApplicationInsights.Common.TelemetryProcessors;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.Common
{
    public static class CommonServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="roleName">Overrides the role name. The default is the assembly name.</param>
        /// <returns></returns>
        public static IServiceCollection EnableApplicationRoles(this IServiceCollection services, string roleName = null)
        {
            // Set application name on Application Insights events
            services.AddSingleton<ITelemetryInitializer>(new ApplicationRoleTelemetryInitializer(roleName));
            return services;
        }

        public static IApplicationBuilder UseCarnegieApplicationInsights(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetService<TelemetryConfiguration>();

            configuration.TelemetryProcessorChainBuilder.Use(next => new SeqLogFilterProcessor(next));

            configuration.TelemetryProcessorChainBuilder.Build();

            return app;
        }
    }
}
