using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator module - range finder
    /// </summary>
    public sealed class RangeFinderDelegate : OperatorDelegateBase<RangeFinderCmd>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public RangeFinderDelegate(IArdNetSystem ArdSys)
            : base(ArdSys, Constants.ChannelNames.TankOperations.RangeFinder)
        {

        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            CmdProxy.SendMessage(RangeFinderCmd.Stop);
        }

        /// <summary>
        /// Send farther command to host
        /// </summary>
        public void AimFarther()
        {
            CmdProxy.SendMessage(RangeFinderCmd.Farther);
        }

        /// <summary>
        /// Send closer command to host
        /// </summary>
        public void AimCloser()
        {
            CmdProxy.SendMessage(RangeFinderCmd.Closer);
        }
    }
}
