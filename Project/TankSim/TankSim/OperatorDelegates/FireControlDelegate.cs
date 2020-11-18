using ArdNet;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator module - fire control
    /// </summary>
    public sealed class FireControlDelegate : OperatorDelegateBase<FireControlCmd>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public FireControlDelegate(IArdNetSystem ArdSys)
            : base(ArdSys, Constants.ChannelNames.TankOperations.FireControl)
        {

        }

        /// <summary>
        /// Send primary fire command to host
        /// </summary>
        public void FirePrimary()
        {
            CmdProxy.SendMessage(FireControlCmd.Primary);
        }

        /// <summary>
        /// Send secondary fire command to host
        /// </summary>
        public void FireSecondary()
        {
            CmdProxy.SendMessage(FireControlCmd.Secondary);
        }
    }
}
