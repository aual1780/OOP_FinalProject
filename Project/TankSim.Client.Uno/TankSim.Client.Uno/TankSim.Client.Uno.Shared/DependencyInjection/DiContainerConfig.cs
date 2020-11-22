using ArdNet;
using ArdNet.Client;
using ArdNet.DependencyInjection;
using ArdNet.TCP;
using J2i.Net.XInputWrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TankSim.Client.GUI.Frames.GameScope;
using TankSim.Client.OperatorModules;
using TankSim.Client.Services;
using TankSim.Client.Uno.Frames.GameScope;

namespace TankSim.Client.Uno
{
    public static class DiContainerConfig
    {
        public static IServiceCollection DefaultServices()
        {
            var services = new ServiceCollection();
            _ = services
                //Add ArdNet
                .AddArdNetClient()
                //setup game services
                .AddGameIDService()

                //TODO: Add operator modules
                //.AddOperatorModules()
                .AddScoped<IOperatorModuleFactory<IOperatorInputModule>, OperatorModuleFactory<IOperatorInputModule>>()
                .AddScoped<IOperatorModuleFactory<IOperatorUIModule>, OperatorModuleFactory<IOperatorUIModule>>()
                .AddScoped<IGamepadService, GamepadService>()
                .AddScoped<IRoleResolverService, RoleResolverService>()
                .AddScoped<IOperatorInputProcessorService, OperatorInputProcessorService>()

                //TODO: Explore xbox compat
                //.AddSingleton((sp) =>
                //{
                //    var xm = XboxControllerManager.GetInstance();
                //    xm.UpdateFrequency = 50;
                //    return xm;
                //})

                //Add main program
                .AddTransient<MainWindowVM>()
                .AddTransient<GameScopeControlVM>()
                //.AddTransient<ClientNameControl>()
                //.AddTransient<ClientNameControlVM>()
                //.AddTransient<OperatorModuleControl>()
                //.AddTransient<OperatorModuleControlVM>()
                ;

            return services;
        }

        private static IServiceCollection AddArdNetClient(this IServiceCollection services)
        {
            var basicConfig = new ArdNetBasicConfig()
            {
                AppID = "ArdNet.TankSim.MultiController",
                ServerPort = 52518,
                ClientPort = 0
            };
            int pingRateMillis = 300;


            _ = services.AddTransient((sp) => Options.Create(basicConfig));
            var builder =
                services.AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet()
                .AddClientScoped()
                .AddConfigModifier((x, y) =>
                {
                    y.TCP.DataSerializationProvider = new MessagePackSerializationProvider();
                    y.TCP.HeartbeatConfig.ForceStrictHeartbeat = true;
                    y.TCP.HeartbeatConfig.RespondToHeartbeats = true;
                    y.TCP.HeartbeatConfig.HeartbeatToleranceMultiplier = 3;
                    y.TCP.HeartbeatConfig.HeartbeatInterval = TimeSpan.FromMilliseconds(pingRateMillis);
                })
                .AddTankSimConfig()
                .AutoRestart()
                ;
            _ = services.AddScoped<IArdNetSystem>((sp) => sp.GetRequiredService<IArdNetClient>());
            return services;
        }
    }
}
