using System;
using System.Collections.Generic;
using System.Text;
using TankSim.Client.GUI.OperatorModules;
using TankSim.Client.OperatorModules;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorServiceExtensions
    {
        public static IServiceCollection AddOperatorModules(this IServiceCollection services)
        {
            _ = services
                .AddTransient<GuiDriver>()
                .AddTransient<FireControlModule>()
                .AddTransient<GunLoaderModule>()
                .AddTransient<GuiGunRotation>()
                .AddTransient<GuiNavigator>()
                .AddTransient<GuiRangeFinder>()
                .AddTransient<GuiDriverCtrl>()
                ;
            return services;
        }
    }
}
