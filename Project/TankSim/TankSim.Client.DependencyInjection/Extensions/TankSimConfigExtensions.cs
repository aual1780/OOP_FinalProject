using ArdNet.Client;
using ArdNet.Client.DependencyInjection;
using ArdNet.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using TankSim;
using TankSim.Client.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class TankSimConfigExtensions

    {
        /// <summary>
        /// Add ArdNet config for tank sim
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static ArdNetInjectionClientBuilder AddTankSimConfig(this ArdNetInjectionClientBuilder Config)
        {
            static ArdNetClientConfig configFactory(IServiceProvider sp)
            {
                var ardConfig = sp.GetRequiredService<IOptions<ArdNetBasicConfig>>().Value;
                var IpResolver = sp.GetRequiredService<IIpResolverService>();
                var gameIdService = sp.GetRequiredService<GameIdService>();
                var gameIdStr = gameIdService.GameID;
                if (!GameIdGenerator.Validate(gameIdStr))
                {
                    throw new InvalidOperationException("Invalid game ID string");
                }
                int gameID = int.Parse(gameIdStr);

                var appID = ardConfig.AppID;
                var myIP = IpResolver.GetIP();
                var serverPort = gameID;
                var clientPort = ardConfig.ClientPort;

                return new ArdNetClientConfig(appID, myIP, serverPort, clientPort);
            }
            _ = Config.AddConfiguration(configFactory);
            return Config;
        }
    }
}
