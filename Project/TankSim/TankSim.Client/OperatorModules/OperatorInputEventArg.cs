using System;
using TIPC.Core.Channels;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Operator input message
    /// </summary>
    public class OperatorInputEventArg : IOperatorInputMsg, IChannelEventArgs
    {
        /// <summary>
        /// Flag switch to determine if a handler has reacted to this message
        /// </summary>
        public bool IsHandled { get; set; } = false;
        /// <summary>
        /// Key data
        /// </summary>
        public ConsoleKeyInfo KeyInfo { get; }
        /// <summary>
        /// UI key input action
        /// </summary>
        public KeyInputType InputType { get; }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="KeyInfo">Key info from input</param>
        /// <param name="InputType">Input action</param>
        public OperatorInputEventArg(ConsoleKeyInfo KeyInfo, KeyInputType InputType)
        {
            this.KeyInfo = KeyInfo;
            this.InputType = InputType;
        }
    }
}
