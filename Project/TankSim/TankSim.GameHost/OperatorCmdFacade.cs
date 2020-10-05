using ArdNet;
using ArdNet.Server;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.GameHost
{
    /// <summary>
    /// Callback delegate to handle driver command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void DriverCmdEventHandler(IConnectedSystemEndpoint Endpoint, DriverCmd Cmd);

    /// <summary>
    /// Operator role facade to encapsulate ArdNet behavior
    /// </summary>
    public class OperatorCmdFacade : IDisposable
    {
        private ITopicMessageProxy<DriverCmd> _driverProxy;

        /// <summary>
        /// Event triggered when driver command is received
        /// </summary>
        public event DriverCmdEventHandler DriverCommandReceived;


        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdServer"></param>
        public OperatorCmdFacade(IArdNetServer ArdServer)
        {
            //TODO: add all modules
            _driverProxy = ArdServer.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
            _driverProxy.MessageReceived += (sender, e)=> DriverCommandReceived?.Invoke(e.SourceEndpoint, e.Message);
        }


        /// <summary>
        /// Release proxy hooks
        /// </summary>
        public void Dispose()
        {
            _driverProxy.Dispose();
        }
    }
}
