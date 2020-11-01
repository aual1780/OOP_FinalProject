using ArdNet.Client;
using ArdNet.Client.DependencyInjection;
using ArdNet.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.DependencyInjection;

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
        public static ArdNetClientConfigurator AddTankSimConfig(this ArdNetClientConfigurator Config)
        {
            ArdNetClientConfig configFactory(IServiceProvider sp)
            {
                var ardConfig = sp.GetRequiredService<IOptions<ArdNetBasicConfig>>().Value;
                var IpResolver = sp.GetRequiredService<IIpResolverService>();
                var gameIdService = sp.GetRequiredService<GameIdService>();
                var gameID = gameIdService.GameID;

                var appID = $"{ardConfig.AppID}.{gameID}";
                var myIP = IpResolver.GetIP();
                var serverPort = ardConfig.ServerPort;
                var clientPort = ardConfig.ClientPort;

                return new ArdNetClientConfig(appID, myIP, serverPort, clientPort);
            }
            _ = Config.AddConfiguration(configFactory);
            return Config;
        }
    }
}
