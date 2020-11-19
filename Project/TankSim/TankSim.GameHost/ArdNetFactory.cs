using ArdNet.Server;
using ArdNet.TCP;
using System;
using TIPC.Core.Channels;
using TIPC.Core.Tools;

namespace TankSim.GameHost
{
    /// <summary>
    /// ArdNet server factory
    /// </summary>
    /// <remarks>
    /// Pattern: Facade
    /// </remarks>
    public static class ArdNetFactory
    {
        /// <summary>
        /// Get new fully configured ardnet server
        /// </summary>
        /// <param name="MsgHub"></param>
        /// <param name="PingRateMills"></param>
        /// <param name="ServerPort"></param>
        /// <returns></returns>
        public static IArdNetServer GetArdServer(MessageHub MsgHub, int PingRateMills = 250, int ServerPort = 0)
        {
            var appID = $"ArdNet.TankSim.MultiController";
            var ipAddr = IPTools.GetLocalIP();
            var config = new ArdNetServerConfig(appID, ipAddr, ServerPort);

            config.TCP.DataSerializationProvider = new MessagepackSerializationProvider();
            config.TCP.HeartbeatConfig.ForceStrictHeartbeat = true;
            config.TCP.HeartbeatConfig.RespondToHeartbeats = true;
            config.TCP.HeartbeatConfig.HeartbeatInterval = TimeSpan.FromMilliseconds(PingRateMills);
            config.TCP.HeartbeatConfig.HeartbeatToleranceMultiplier = 3;

            var ardServer = new ArdNetServer(config, MsgHub);
            ardServer.Start();
            return ardServer;
        }

    }
}
