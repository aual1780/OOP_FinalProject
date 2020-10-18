using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - driver
    /// </summary>
    internal sealed class NavigatorDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<NavigatorCmd> _cmdProxy;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public NavigatorDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(NavigatorCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            _cmdProxy.SendMessage(NavigatorCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            _cmdProxy.SendMessage(NavigatorCmd.Right);
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
