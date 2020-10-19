using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle gun loader command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void GunLoaderCmdEventHandler(IConnectedSystemEndpoint Endpoint, GunLoaderCmd Cmd);


    /// <summary>
    /// Operator module - gun loader
    /// </summary>
    public sealed class GunLoaderDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<GunLoaderCmd> _cmdProxy;
        private readonly object _cmdHandlerLock = new object();
        private GunLoaderCmdEventHandler _cmdHandler;

        /// <summary>
        /// 
        /// </summary>
        public event GunLoaderCmdEventHandler CmdReceived
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

        private void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<GunLoaderCmd> e)
        {
            _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public GunLoaderDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<GunLoaderCmd>(Constants.ChannelNames.TankOperations.GunLoader);
        }

        /// <summary>
        /// Send load command to host
        /// </summary>
        public void Load()
        {
            _cmdProxy.SendMessage(GunLoaderCmd.Load);
        }

        /// <summary>
        /// Send cycle ammo command to host
        /// </summary>
        public void CycleAmmoType()
        {
            _cmdProxy.SendMessage(GunLoaderCmd.CycleAmmoType);
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
