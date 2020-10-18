using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - fire control
    /// </summary>
    internal sealed class FireControlDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<FireControlCmd> _cmdProxy;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public FireControlDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<FireControlCmd>(Constants.ChannelNames.TankOperations.FireControl);
        }

        /// <summary>
        /// Send primary fire command to host
        /// </summary>
        public void FirePrimary()
        {
            _cmdProxy.SendMessage(FireControlCmd.Primary);
        }

        /// <summary>
        /// Send secondary fire command to host
        /// </summary>
        public void FireSecondary()
        {
            _cmdProxy.SendMessage(FireControlCmd.Secondary);
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
