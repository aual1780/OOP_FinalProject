using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle movement commands
    /// </summary>
    /// <param name="Endpoint"></param>
    /// <param name="Dir"></param>
    public delegate void TankMovementCmdEventHandler(IConnectedSystemEndpoint Endpoint, MovementDirection Dir);


    /// <summary>
    /// Delegate for tracking tank true movement direction
    /// </summary>
    public sealed class TankMovementDelegate : IDisposable
    {
        const MovementDirection _ns = (MovementDirection.North | MovementDirection.South);
        const MovementDirection _ew = (MovementDirection.East | MovementDirection.West);
        private MovementDirection _dir;
        readonly object _nsLock = new object();
        readonly object _ewLock = new object();
        private readonly ITopicMessageProxy<DriverCmd> _driveProxy;
        private readonly ITopicMessageProxy<NavigatorCmd> _navProxy;
        private readonly object _cmdHandlerLock = new object();
        private TankMovementCmdEventHandler _cmdHandler;

        /// <summary>
        /// Event preprocess validator.
        /// Use to intercept events and prevent bubbling
        /// </summary>
        public DelegateEventValidator<(IConnectedSystemEndpoint Endpt, MovementDirection Dir)> Validator
        {
            get;
        } = new DelegateEventValidator<(IConnectedSystemEndpoint Endpt, MovementDirection Dir)>();


        /// <summary>
        /// Event triggered when movement direction is changed
        /// </summary>
        public event TankMovementCmdEventHandler MovementChanged
        {
            add
            {
                lock (_cmdHandlerLock)
                {
                    _cmdHandler += value;
                    if (_cmdHandler != null)
                    {
                        _driveProxy.MessageReceived += DriveProxy_MessageReceived;
                        _navProxy.MessageReceived += NavProxy_MessageReceived;
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
                        _driveProxy.MessageReceived -= DriveProxy_MessageReceived;
                    }
                }
            }
        }

        void DriveProxy_MessageReceived(
            object Sender,
            TopicProxyMessageEventArgs<DriverCmd> e)
        {
            var tup = (e.SourceEndpoint, (MovementDirection)e.Message.Direction);
            if (!Validator.Validate(tup))
            {
                return;
            }
            MovementDirection dirCopy = 0;
            lock (_nsLock)
            {
                switch (e.Message.Direction)
                {
                    case DriveDirection.Stop:
                        _dir &= _ew;
                        break;
                    case DriveDirection.Forward:
                        _dir &= _ew;
                        _dir |= MovementDirection.North;
                        break;
                    case DriveDirection.Backward:
                        _dir &= _ew;
                        _dir |= MovementDirection.South;
                        break;
                }
                dirCopy = _dir;
            }
            _cmdHandler?.Invoke(e.SourceEndpoint, dirCopy);
        }

        void NavProxy_MessageReceived(
            object Sender,
            TopicProxyMessageEventArgs<NavigatorCmd> e)
        {
            var tup = (e.SourceEndpoint, (MovementDirection)e.Message.Direction);
            if (!Validator.Validate(tup))
            {
                return;
            }
            MovementDirection dirCopy = 0;
            lock (_ewLock)
            {
                switch (e.Message.Direction)
                {
                    case RotationDirection.Stop:
                        _dir &= _ns;
                        break;
                    case RotationDirection.Left:
                        _dir &= _ns;
                        _dir |= MovementDirection.West;
                        break;
                    case RotationDirection.Right:
                        _dir &= _ns;
                        _dir |= MovementDirection.East;
                        break;
                }
                dirCopy = _dir;
            }
            _cmdHandler?.Invoke(e.SourceEndpoint, dirCopy);
        }


        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public TankMovementDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _driveProxy = ArdSys.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
            _navProxy = ArdSys.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _cmdHandler = null;
            _driveProxy.Dispose();
            _navProxy.Dispose();
        }
    }
}
