using ArdNet;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class NavigatorDelegate : OperatorDelegateBase<NavigatorCmd>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public NavigatorDelegate(IArdNetSystem ArdSys)
            : base(ArdSys, Constants.ChannelNames.TankOperations.Navigator)
        {

        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            CmdProxy.SendMessage(NavigatorCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            CmdProxy.SendMessage(NavigatorCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            CmdProxy.SendMessage(NavigatorCmd.Right);
        }
    }
}
