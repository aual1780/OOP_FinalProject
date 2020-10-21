using ArdNet.Server;
using System;
using System.Collections.Generic;
using System.Text;
using TIPC.Core.Channels;
using TIPC.Core.Tools;

namespace TankSim.GameHost
{
    /// <summary>
    /// ArdNet server factory
    /// </summary>
    public static class ArdNetFactory
    {
        /// <summary>
        /// Get new fully configured ardnet server
        /// </summary>
        /// <param name="MsgHub"></param>
        /// <param name="GameID"></param>
        /// <param name="PingRateMills"></param>
        /// <param name="ServerPort"></param>
        /// <returns></returns>
        public static IArdNetServer GetArdServer(MessageHub MsgHub, string GameID = "", int PingRateMills = 250, int ServerPort = 52518)
        {
            if (string.IsNullOrWhiteSpace(GameID))
            {
                GameID = GameIdGenerator.GetID();
            }
            if (!GameIdGenerator.Validate(GameID))
            {
                throw new ArgumentException(nameof(GameID));
            }

            var appID = $"ArdNet.TankSim.MultiController.{GameID}";
            var ipAddr = IPTools.GetLocalIP();
            var config = new ArdNetServerConfig(appID, ipAddr, ServerPort);

            config.TCP.HeartbeatConfig.ForceStrictHeartbeat = true;
            config.TCP.HeartbeatConfig.RespondToHeartbeats = false;
            config.TCP.HeartbeatConfig.HeartbeatInterval = TimeSpan.FromMilliseconds(PingRateMills);

            var ardServer = new ArdNetServer(config, MsgHub);
            ardServer.Start();
            return ardServer;
        }

    }
}
