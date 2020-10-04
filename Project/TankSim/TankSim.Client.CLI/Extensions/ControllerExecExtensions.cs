using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TankSim.Client.CLI.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ControllerExecExtensions
    {
        public static IServiceCollection AddControllerExecService(this IServiceCollection services)
        {
            _= services.AddScoped<ControllerExecService>();
            return services;
        }
    }
}
