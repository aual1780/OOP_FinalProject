using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class DriverDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<DriverCmd> _cmdProxy;

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
