using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Delegate for tracking tank true movement direction
    /// </summary>
    public sealed class TankAimingDelegate : IDisposable
    {
        const MovementDirection _ns = (MovementDirection.North | MovementDirection.South);
        const MovementDirection _ew = (MovementDirection.East | MovementDirection.West);
        private MovementDirection _dir;
        readonly object _nsLock = new();
        readonly object _ewLock = new();
        private readonly ITopicMessageProxy<RangeFinderCmd> _rangeFinderProxy;
        private readonly ITopicMessageProxy<GunRotationCmd> _gunRotProxy;
        private readonly object _cmdHandlerLock = new();
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
        public event TankMovementCmdEventHandler AimChanged
        {
            add
            {
                lock (_cmdHandlerLock)
                {
                    _cmdHandler += value;
                    if (_cmdHandler != null)
                    {
                        _rangeFinderProxy.MessageReceived += DriveProxy_MessageReceived;
                        _gunRotProxy.MessageReceived += NavProxy_MessageReceived;
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
                        _rangeFinderProxy.MessageReceived -= DriveProxy_MessageReceived;
                    }
                }
            }
        }

        void DriveProxy_MessageReceived(
            object Sender,
            TopicProxyMessageEventArgs<RangeFinderCmd> e)
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
                    case RangeDirection.Stop:
                        _dir &= _ew;
                        break;
                    case RangeDirection.Farther:
                        _dir &= _ew;
                        _dir |= MovementDirection.North;
                        break;
                    case RangeDirection.Closer:
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
            TopicProxyMessageEventArgs<GunRotationCmd> e)
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
        public TankAimingDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _rangeFinderProxy = ArdSys.TopicManager.GetProxy<RangeFinderCmd>(Constants.ChannelNames.TankOperations.RangeFinder);
            _gunRotProxy = ArdSys.TopicManager.GetProxy<GunRotationCmd>(Constants.ChannelNames.TankOperations.GunRotation);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _cmdHandler = null;
            _rangeFinderProxy.Dispose();
            _gunRotProxy.Dispose();
        }
    }
}
