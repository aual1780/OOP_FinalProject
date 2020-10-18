using ArdNet;
using ArdNet.Topics;
using System;
using TankSim.OperatorCmds;

namespace TankSim.Client.OperatorDelegates
{
    /// <summary>
    /// Operator module - range finder
    /// </summary>
    public sealed class RangeFinderDelegate : IDisposable
    {
        private readonly ITopicMessageProxy<RangeFinderCmd> _cmdProxy;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="ArdSys"></param>
        public RangeFinderDelegate(IArdNetSystem ArdSys)
        {
            if (ArdSys is null)
            {
                throw new ArgumentNullException(nameof(ArdSys));
            }

            _cmdProxy = ArdSys.TopicManager.GetProxy<RangeFinderCmd>(Constants.ChannelNames.TankOperations.RangeFinder);
        }

        /// <summary>
        /// Send stop command to host
        /// </summary>
        public void Stop()
        {
            _cmdProxy.SendMessage(RangeFinderCmd.Stop);
        }

        /// <summary>
        /// Send farther command to host
        /// </summary>
        public void AimFarther()
        {
            _cmdProxy.SendMessage(RangeFinderCmd.Farther);
        }

        /// <summary>
        /// Send closer command to host
        /// </summary>
        public void AimCloser()
        {
            _cmdProxy.SendMessage(RangeFinderCmd.Closer);
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
