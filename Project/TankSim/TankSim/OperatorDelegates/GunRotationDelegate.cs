using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle gun rotation command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void GunRotationCmdEventHandler(IConnectedSystemEndpoint Endpoint, GunRotationCmd Cmd);

    /// <summary>
    /// Operator module - gun rotation
    /// </summary>
    public sealed class GunRotationDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<GunRotationCmd> _cmdProxy;
        private readonly object _cmdHandlerLock = new object();
        private GunRotationCmdEventHandler _cmdHandler;

        /// <summary>
        /// 
        /// </summary>
        public event GunRotationCmdEventHandler CmdReceived
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

        private void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<GunRotationCmd> e)
        {
            _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public GunRotationDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<GunRotationCmd>(Constants.ChannelNames.TankOperations.GunRotation);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(GunRotationCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            _cmdProxy.SendMessage(GunRotationCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            _cmdProxy.SendMessage(GunRotationCmd.Right);
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
