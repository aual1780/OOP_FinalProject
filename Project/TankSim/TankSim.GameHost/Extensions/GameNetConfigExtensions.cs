using ArdNet.Server;
using ArdNet.Server.DependencyInjection;
using ArdNet.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TankSim;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameNetConfigExtensions
    {
        /// <summary>
        /// Add ArdNet config for tank sim
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static ArdNetServerConfigurator AddTankSimConfig(this ArdNetServerConfigurator Config)
        {
            ArdNetServerConfig configFactory(IServiceProvider sp)
            {
                var ardConfig = sp.GetRequiredService<IOptions<ArdNetBasicConfig>>().Value;
                var IpResolver = sp.GetRequiredService<IIpResolverService>();
                var gameID = GameIdGenerator.GetID();

                var appID = $"{ardConfig.AppID}.{gameID}";
                var myIP = IpResolver.GetIP();
                var serverPort = ardConfig.ServerPort;

                return new ArdNetServerConfig(appID, myIP, serverPort);
            }
            _ = Config.AddConfiguration(configFactory);
            return Config;
        }
    }
}
