using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - driver
    /// </summary>
    public sealed class NavigatorDelegate : IDisposable
    {
        private ITopicMessageProxy<NavigatorCmd> NavCmdProxy { get; }

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

            NavCmdProxy = ArdSys.TopicManager.GetProxy<NavigatorCmd>(Constants.ChannelNames.TankOperations.Navigator);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            NavCmdProxy.SendMessage(NavigatorCmd.Stop);
        }

        /// <summary>
        /// Send left command to host
        /// </summary>
        public void TurnLeft()
        {
            NavCmdProxy.SendMessage(NavigatorCmd.Left);
        }

        /// <summary>
        /// Send right command to host
        /// </summary>
        public void TurnRight()
        {
            NavCmdProxy.SendMessage(NavigatorCmd.Right);
        }

        /// <summary>
        /// Unhook topics
        /// </summary>
        public void Dispose()
        {
            NavCmdProxy.Dispose();
        }

    }
}
