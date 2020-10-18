using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - gun rotation
    /// </summary>
    internal sealed class GunRotationDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<GunRotationCmd> _cmdProxy;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public GunRotationDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<GunRotationCmd>(Constants.ChannelNames.TankOperations.GunRotation);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(GunRotationCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            _cmdProxy.SendMessage(GunRotationCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            _cmdProxy.SendMessage(GunRotationCmd.Right);
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
