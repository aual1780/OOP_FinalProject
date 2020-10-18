using System;
using System.Collections.Generic;
using System.Text;
using TankSim.Client.GUI.OperatorModules;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorServiceExtensions
    {
        public static IServiceCollection AddOperatorModules(this IServiceCollection services)
        {
            _ = services
                .AddTransient<GuiDriver>()
                .AddTransient<GuiFireControl>()
                .AddTransient<GuiGunLoader>()
                .AddTransient<GuiGunRotation>()
                .AddTransient<GuiNavigator>()
                .AddTransient<GuiRangeFinder>()
                .AddTransient<GuiDriverCtrl>()
                ;
            return services;
        }
    }
}
