using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle nav command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void NavigatorCmdEventHandler(IConnectedSystemEndpoint Endpoint, NavigatorCmd Cmd);

    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class NavigatorDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<NavigatorCmd> _cmdProxy;
        private readonly object _cmdHandlerLock = new object();
        private NavigatorCmdEventHandler _cmdHandler;

        /// <summary>
        /// 
        /// </summary>
        public event NavigatorCmdEventHandler CmdReceived
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

        private void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<NavigatorCmd> e)
        {
            _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public NavigatorDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(NavigatorCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            _cmdProxy.SendMessage(NavigatorCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            _cmdProxy.SendMessage(NavigatorCmd.Right);
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
