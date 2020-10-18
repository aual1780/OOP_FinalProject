using ArdNet;
using ArdNet.Topics;
using System;
using System.Collections.Generic;
using System.Text;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Callback delegate to handle movement commands
    /// </summary>
    /// <param name="Dir"></param>
    public delegate void TankMovementCmdEventHandler(MovementDirection Dir);


    /// <summary>
    /// Delegate for tracking tank true movement direction
    /// </summary>
    public sealed class TankMovementDelegate : IDisposable
    {
        private MovementDirection _dir;
        private readonly ITopicMessageProxy<DriverCmd> _driveProxy;
        private readonly ITopicMessageProxy<NavigatorCmd> _navProxy;

        /// <summary>
        /// Event triggered when movement direction is changed
        /// </summary>
        public event TankMovementCmdEventHandler MovementChanged;

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

            var ns = (MovementDirection.North | MovementDirection.South);
            var ew = (MovementDirection.East | MovementDirection.West);
            var nsLock = new object();
            var ewLock = new object();

            _driveProxy = ArdSys.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
            _driveProxy.MessageReceived += (sender, arg) =>
            {
                MovementDirection dirCopy = 0;
                lock (nsLock)
                {
                    switch (arg.Message.Direction)
                    {
                        case DriveDirection.Stop:
                            _dir &= ew;
                            break;
                        case DriveDirection.Forward:
                            _dir &= ew;
                            _dir |= MovementDirection.North;
                            break;
                        case DriveDirection.Backward:
                            _dir &= ew;
                            _dir |= MovementDirection.South;
                            break;
                    }
                    dirCopy = _dir;
                }
                MovementChanged?.Invoke(dirCopy);
            };
            _navProxy = ArdSys.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
            _navProxy.MessageReceived += (sender, arg) =>
            {
                MovementDirection dirCopy = 0;
                lock (ewLock)
                {
                    switch (arg.Message.Direction)
                    {
                        case RotationDirection.Stop:
                            _dir &= ns;
                            break;
                        case RotationDirection.Left:
                            _dir &= ns;
                            _dir |= MovementDirection.West;
                            break;
                        case RotationDirection.Right:
                            _dir &= ns;
                            _dir |= MovementDirection.East;
                            break;
                    }
                    dirCopy = _dir;
                }
                MovementChanged?.Invoke(dirCopy);
            };
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _driveProxy?.Dispose();
            _navProxy?.Dispose();
        }
    }
}
