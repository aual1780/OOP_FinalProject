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
    /// Callback delegate to handle nav command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void NavigatorCmdEventHandler(IConnectedSystemEndpoint Endpoint, NavigatorCmd Cmd);

    /// <summary>
    /// Operator role facade to encapsulate ArdNet behavior
    /// </summary>
    public class OperatorCmdFacade : IDisposable
    {
        private readonly List<ITopicProxy> _proxySet = new List<ITopicProxy>();

        /// <summary>
        /// Event triggered when driver command is received
        /// </summary>
        public event DriverCmdEventHandler DriverCommandReceived;

        /// <summary>
        /// Event triggered when nav command is received
        /// </summary>
        public event NavigatorCmdEventHandler NavigatorCommandReceived;


        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdServer"></param>
        public OperatorCmdFacade(IArdNetServer ArdServer)
        {
            //TODO: add all modules
            {
                var proxy = ArdServer.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
                proxy.MessageReceived += (sender, e) => DriverCommandReceived?.Invoke(e.SourceEndpoint, e.Message);
                _proxySet.Add(proxy);
            }
            {
                var proxy = ArdServer.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
                proxy.MessageReceived += (sender, e) => NavigatorCommandReceived?.Invoke(e.SourceEndpoint, e.Message);
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
