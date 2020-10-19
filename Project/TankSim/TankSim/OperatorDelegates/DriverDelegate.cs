using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle driver command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void DriverCmdEventHandler(IConnectedSystemEndpoint Endpoint, DriverCmd Cmd);


    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class DriverDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<DriverCmd> _cmdProxy;
        private readonly object _cmdHandlerLock = new object();
        private DriverCmdEventHandler _cmdHandler;

        /// <summary>
        /// 
        /// </summary>
        public event DriverCmdEventHandler CmdReceived
        {
            add
            {
                lock (_cmdHandlerLock)
                {
                    _cmdHandler += value;
                    if (_cmdHandler != null)
                    {
                        _cmdProxy.MessageReceived += CmdProxy_MessageReceived;
                    }
                }
            }
            remove
            {
                lock (_cmdHandlerLock)
                {
                    _cmdHandler -= value;
                    if (_cmdHandler == null)
                    {
                        _cmdProxy.MessageReceived -= CmdProxy_MessageReceived;
                    }
                }
            }
        }

        private void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<DriverCmd> e)
        {
            _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
        }


        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public DriverDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(DriverCmd.Stop);
        }

        /// <summary>
        /// Send forward command to host
        /// </summary>
        public void DriveForward()
        {
            _cmdProxy.SendMessage(DriverCmd.Forward);
        }

        /// <summary>
        /// Send backward command to host
        /// </summary>
        public void DriveBackward()
        {
            _cmdProxy.SendMessage(DriverCmd.Backward);
        }

        /// <summary>
        /// Unhook topics
        /// </summary>
        public void Dispose()
        {
            _cmdProxy.Dispose();
        }

    }
}
