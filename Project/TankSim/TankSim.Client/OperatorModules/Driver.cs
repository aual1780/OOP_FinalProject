using ArdNet;
using ArdNet.Topics;
using System;
using System.Collections.Generic;
using System.Text;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class Driver : IOpModule
    {
        private IArdNetSystem ArdSys { get; }
        private ITopicMessageProxy<DriverCmd> DriveCmdProxy { get; }

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public Driver(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            this.ArdSys = ArdSys;
            DriveCmdProxy = ArdSys.TopicManager.GetProxy<DriverCmd>(Constants.ChannelNames.TankOperations.Driver);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            DriveCmdProxy.SendMessage(DriverCmd.Stop);
        }

        /// <summary>
        /// Send forward command to host
        /// </summary>
        public void DriveForward()
        {
            DriveCmdProxy.SendMessage(DriverCmd.Forward);
        }

        /// <summary>
        /// Send backward command to host
        /// </summary>
        public void DriveBackward()
        {
            DriveCmdProxy.SendMessage(DriverCmd.Backward);
        }

        /// <summary>
        /// Unhook topics
        /// </summary>
        public void Dispose()
        {
            DriveCmdProxy.Dispose();
        }

    }
}
