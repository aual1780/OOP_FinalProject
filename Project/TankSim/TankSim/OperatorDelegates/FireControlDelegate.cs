using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle fire control command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void FireControlCmdEventHandler(IConnectedSystemEndpoint Endpoint, FireControlCmd Cmd);


    /// <summary>
    /// Operator module - fire control
    /// </summary>
    public sealed class FireControlDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<FireControlCmd> _cmdProxy;
        private readonly object _cmdHandlerLock = new object();
        private FireControlCmdEventHandler _cmdHandler;

        /// <summary>
        /// 
        /// </summary>
        public event FireControlCmdEventHandler CmdReceived
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

        private void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<FireControlCmd> e)
        {
            _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public FireControlDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<FireControlCmd>(Constants.ChannelNames.TankOperations.FireControl);
        }

        /// <summary>
        /// Send primary fire command to host
        /// </summary>
        public void FirePrimary()
        {
            _cmdProxy.SendMessage(FireControlCmd.Primary);
        }

        /// <summary>
        /// Send secondary fire command to host
        /// </summary>
        public void FireSecondary()
        {
            _cmdProxy.SendMessage(FireControlCmd.Secondary);
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
