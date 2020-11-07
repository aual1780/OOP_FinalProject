﻿using ArdNet;
using ArdNet.Client;
using ArdNet.Client.DependencyInjection;
using ArdNet.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArdNetSvcExtensions
    {
        /// <summary>
        /// Add an immutable keybind config to the service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static ArdNetClientConfigurator AddArdNetClient(this IServiceCollection services, IConfiguration Config)
        {
            //Pattern: Builder
            //Pattern: DI
            //Pattern: Strategy
            services.Configure<ArdNetBasicConfig>(Config.GetSection("ArdNet").GetSection(nameof(ArdNetBasicConfig)));
            var configurator = services.AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet()
                .AddClientScoped()
                .AddConfigModifier((x, y) =>
                {
                    y.TCP.HeartbeatConfig.ForceStrictHeartbeat = true;
                    y.TCP.HeartbeatConfig.RespondToHeartbeats = true;
                    y.TCP.HeartbeatConfig.HeartbeatToleranceMultiplier = 3;
                    var pingRate = Config.GetValue<int>("ArdNet:ArdNetBasicConfig:PingRateMillis");
                    y.TCP.HeartbeatConfig.HeartbeatInterval = TimeSpan.FromMilliseconds(pingRate);
                })
                .AddTankSimConfig()
                .AutoRestart()
                ;
            _ = services.AddScoped<IArdNetSystem>((sp) => sp.GetRequiredService<IArdNetClient>());
            return configurator;
        }
    }
}