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
                .AddTransient<DriverModule>()
                .AddTransient<FireControlModule>()
                .AddTransient<GunLoaderModule>()
                .AddTransient<GunRotationModule>()
                .AddTransient<NavigatorModule>()
                .AddTransient<RangeFinderModule>()
                .AddTransient<GuiDriverCtrl>()
                .AddTransient<GuiFireControlCtrl>()
                .AddTransient<GuiGunLoaderCtrl>()
                .AddTransient<GuiGunRotationCtrl>()
                .AddTransient<GuiNavigatorCtrl>()
                .AddTransient<GuiRangeFinderCtrl>()
                ;
            return services;
        }
    }
}
