using ArdNet;
using ArdNet.Topics;
using System;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle genreric operator command
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Cmd"></param>
    public delegate void OperatorCmdEventHandler<T_Data>(IConnectedSystemEndpoint Endpoint, T_Data Cmd)
        where T_Data : class;

    /// <summary>
    /// Base class for operator delegates
    /// </summary>
    /// <typeparam name="T_Data"></typeparam>
    public class OperatorDelegateBase<T_Data> : IDisposable
        where T_Data : class
    {
        private readonly object _cmdHandlerLock = new object();
        private OperatorCmdEventHandler<T_Data> _cmdHandler;
        /// <summary>
        /// ArdNet cmd proxy
        /// </summary>
        protected ITopicMessageProxy<T_Data> CmdProxy { get; }

        /// <summary>
        /// Event preprocess validator.
        /// Use to intercept events and prevent bubbling
        /// </summary>
        public DelegateEventValidator<TopicProxyMessageEventArgs<T_Data>> Validator { get; }
            = new DelegateEventValidator<TopicProxyMessageEventArgs<T_Data>>();


        /// <summary>
        /// Event triggered when a command is received on this channel
        /// </summary>
        /// <remarks>
        /// Hook/unhook underlying CmdProxy.MessageReceived event
        /// This allows us to utilise the built-in ArdNet sub/unsub feature
        /// We will only get cmd messages from the server if a client is processing them
        /// </remarks>
        public event OperatorCmdEventHandler<T_Data> CmdReceived
        {
            add
            {
                lock (_cmdHandlerLock)
                {
                    _cmdHandler += value;
                    if (_cmdHandler != null)
                    {
                        CmdProxy.MessageReceived += CmdProxy_MessageReceived;
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
                        CmdProxy.MessageReceived -= CmdProxy_MessageReceived;
                    }
                }
            }
        }

        void CmdProxy_MessageReceived(object Sender, TopicProxyMessageEventArgs<T_Data> e)
        {
            if (Validator.Validate(e))
            {
                _cmdHandler?.Invoke(e.SourceEndpoint, e.Message);
            }
        }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdSys"></param>
        /// <param name="ChannelName"></param>
        public OperatorDelegateBase(IArdNetSystem ArdSys, string ChannelName)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            CmdProxy = ArdSys.TopicManager.GetProxy<T_Data>(ChannelName);
        }


        /// <summary>
        /// Unhook topics
        /// </summary>
        public void Dispose()
        {
            _cmdHandler = null;
            CmdProxy.Dispose();
        }

    }
}
