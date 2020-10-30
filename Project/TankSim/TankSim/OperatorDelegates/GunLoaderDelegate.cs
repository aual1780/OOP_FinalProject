using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.OperatorDelegates
{
    /// <summary>
    /// Operator module - gun loader
    /// </summary>
    public sealed class GunLoaderDelegate : OperatorDelegateBase<GunLoaderCmd>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public GunLoaderDelegate(IArdNetSystem ArdSys)
            : base(ArdSys, Constants.ChannelNames.TankOperations.GunLoader)
        {

        }

        /// <summary>
        /// Send load command to host
        /// </summary>
        public void Load()
        {
            CmdProxy.SendMessage(GunLoaderCmd.Load);
        }

        /// <summary>
        /// Send cycle ammo command to host
        /// </summary>
        public void CycleAmmoType()
        {
            CmdProxy.SendMessage(GunLoaderCmd.CycleAmmoType);
        }
    }
}
