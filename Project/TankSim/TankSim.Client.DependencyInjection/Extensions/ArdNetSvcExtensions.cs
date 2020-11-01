using ArdNet;
using ArdNet.Client;
using ArdNet.Client.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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
            var configurator = services.AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet(Config.GetSection("ArdNet"))
                .AddClientScoped()
                .AddConfigModifier((x, y) =>
                {
                    y.TCP.HeartbeatConfig.ForceStrictHeartbeat = false;
                    y.TCP.HeartbeatConfig.RespondToHeartbeats = false;
                    var pingRate = Config.GetValue<int>("ArdNet.PingRateMillis") + 50;
                    y.TCP.HeartbeatConfig.HeartbeatInterval = TimeSpan.FromMilliseconds(pingRate);
                })
                .AddTankSimConfig()
                .AutoRestart();
            _ = services.AddScoped<IArdNetSystem>((sp) => sp.GetRequiredService<IArdNetClient>());
            return configurator;
        }
    }
}
