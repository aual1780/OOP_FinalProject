using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class DriverDelegate : OperatorDelegateBase<DriverCmd>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public DriverDelegate(IArdNetSystem ArdSys) 
            : base(ArdSys, Constants.ChannelNames.TankOperations.Driver)
        {
            
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            CmdProxy.SendMessage(DriverCmd.Stop);
        }

        /// <summary>
        /// Send forward command to host
        /// </summary>
        public void DriveForward()
        {
            CmdProxy.SendMessage(DriverCmd.Forward);
        }

        /// <summary>
        /// Send backward command to host
        /// </summary>
        public void DriveBackward()
        {
            CmdProxy.SendMessage(DriverCmd.Backward);
        }

    }
}
