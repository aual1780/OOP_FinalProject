using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle range command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void RangeFinderCmdEventHandler(IConnectedSystemEndpoint Endpoint, RangeFinderCmd Cmd);

    /// <summary>
    /// Operator module - range finder
    /// </summary>
    public sealed class RangeFinderDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<RangeFinderCmd> _cmdProxy;
        private readonly object _cmdHandlerLock = new object();
        private RangeFinderCmdEventHandler _cmdHandler;

        /// <summary>
        /// 
        /// </summary>
        public event RangeFinderCmdEventHandler CmdReceived
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

        private void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<RangeFinderCmd> e)
        {
            _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public RangeFinderDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<RangeFinderCmd>(Constants.ChannelNames.TankOperations.RangeFinder);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(RangeFinderCmd.Stop);
        }

        /// <summary>
        /// Send farther command to host
        /// </summary>
        public void AimFarther()
        {
            _cmdProxy.SendMessage(RangeFinderCmd.Farther);
        }

        /// <summary>
        /// Send closer command to host
        /// </summary>
        public void AimCloser()
        {
            _cmdProxy.SendMessage(RangeFinderCmd.Closer);
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
