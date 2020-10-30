using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator module - gun rotation
    /// </summary>
    public sealed class GunRotationDelegate : OperatorDelegateBase<GunRotationCmd>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public GunRotationDelegate(IArdNetSystem ArdSys)
            : base(ArdSys, Constants.ChannelNames.TankOperations.GunRotation)
        {

        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            CmdProxy.SendMessage(GunRotationCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            CmdProxy.SendMessage(GunRotationCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            CmdProxy.SendMessage(GunRotationCmd.Right);
        }
    }
}
