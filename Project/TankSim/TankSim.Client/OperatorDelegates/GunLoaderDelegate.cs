using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - gun loader
    /// </summary>
    internal sealed class GunLoaderDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<GunLoaderCmd> _cmdProxy;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public GunLoaderDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<GunLoaderCmd>(Constants.ChannelNames.TankOperations.GunLoader);
        }

        /// <summary>
        /// Send load command to host
        /// </summary>
        public void Load()
        {
            _cmdProxy.SendMessage(GunLoaderCmd.Load);
        }

        /// <summary>
        /// Send cycle ammo command to host
        /// </summary>
        public void CycleAmmoType()
        {
            _cmdProxy.SendMessage(GunLoaderCmd.CycleAmmoType);
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
