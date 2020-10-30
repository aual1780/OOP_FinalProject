using TankSim.Client.GUI.Frames.Operations;
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
                .AddTransient<GuiDriver>()
                .AddTransient<GuiDriverVM>()
                .AddTransient<GuiGunLoaderCtrl>()
                .AddTransient<GuiGunLoaderVM>()
                .AddTransient<GuiGunAimCtrl>()
                .AddTransient<GuiGunAimVM>()
                ;
            return services;
        }
    }
}
