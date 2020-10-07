using ArdNet;
using ArdNet.Server;
using ArdNet.Topics;
using System;
using System.Collections.Generic;
using TankSim.OperatorCmds;
using TIPC.Core.Tools.Extensions.IEnumerable;

namespace TankSim.GameHost
{
    /// <summary>
    /// Callback delegate to handle driver command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void DriverCmdEventHandler(IConnectedSystemEndpoint Endpoint, DriverCmd Cmd);

    /// <summary>
    /// Callback delegate to handle fire control command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void FireControlCmdEventHandler(IConnectedSystemEndpoint Endpoint, FireControlCmd Cmd);

    /// <summary>
    /// Callback delegate to handle gun loader command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void GunLoaderCmdEventHandler(IConnectedSystemEndpoint Endpoint, GunLoaderCmd Cmd);

    /// <summary>
    /// Callback delegate to handle gun rotation command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void GunRotationCmdEventHandler(IConnectedSystemEndpoint Endpoint, GunRotationCmd Cmd);

    /// <summary>
    /// Callback delegate to handle nav command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void NavigatorCmdEventHandler(IConnectedSystemEndpoint Endpoint, NavigatorCmd Cmd);

    /// <summary>
    /// Callback delegate to handle range command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void RangeFinderCmdEventHandler(IConnectedSystemEndpoint Endpoint, RangeFinderCmd Cmd);

    /// <summary>
    /// Operator role facade to encapsulate ArdNet behavior
    /// </summary>
    public class OperatorCmdFacade : IDisposable
    {
        private readonly List<ITopicProxy> _proxySet = new List<ITopicProxy>();

        /// <summary>
        /// Event triggered when driver command is received
        /// </summary>
        public event DriverCmdEventHandler DriverCmdReceived;

        /// <summary>
        /// Event triggered when fire control command is received
        /// </summary>
        public event FireControlCmdEventHandler FireControlCmdReceived;

        /// <summary>
        /// Event triggered when gun loader command is received
        /// </summary>
        public event GunLoaderCmdEventHandler GunLoaderCmdReceived;

        /// <summary>
        /// Event triggered when gun rotation command is received
        /// </summary>
        public event GunRotationCmdEventHandler GunRotationCmdReceived;

        /// <summary>
        /// Event triggered when navigator command is received
        /// </summary>
        public event NavigatorCmdEventHandler NavigatorCmdReceived;

        /// <summary>
        /// Event triggered when range finder command is received
        /// </summary>
        public event RangeFinderCmdEventHandler RangeFinderCmdReceived;


        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdServer"></param>
        public OperatorCmdFacade(IArdNetServer ArdServer)
        {
            {
                var proxy = ArdServer.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
                proxy.MessageReceived += (sender, e) => DriverCmdReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
            {
                var proxy = ArdServer.TopicManager.GetProxy<FireControlCmd>(Constants.ChannelNames.TankOperations.FireControl);
                proxy.MessageReceived += (sender, e) => FireControlCmdReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
            {
                var proxy = ArdServer.TopicManager.GetProxy<GunLoaderCmd>(Constants.ChannelNames.TankOperations.GunLoader);
                proxy.MessageReceived += (sender, e) => GunLoaderCmdReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
            {
                var proxy = ArdServer.TopicManager.GetProxy<GunRotationCmd>(Constants.ChannelNames.TankOperations.GunRotation);
                proxy.MessageReceived += (sender, e) => GunRotationCmdReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
            {
                var proxy = ArdServer.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
                proxy.MessageReceived += (sender, e) => NavigatorCmdReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
            {
                var proxy = ArdServer.TopicManager.GetProxy<RangeFinderCmd>(Constants.ChannelNames.TankOperations.RangeFinder);
                proxy.MessageReceived += (sender, e) => RangeFinderCmdReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
        }


        /// <summary>
        /// Release proxy hooks
        /// </summary>
        public void Dispose()
        {
            _proxySet.DisposeAll();
        }
    }
}
