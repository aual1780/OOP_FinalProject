using System;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Operator input message
    /// </summary>
    public interface IOperatorInputMsg
    {
        /// <summary>
        /// Flag switch to determine if a handler has reacted to this message
        /// </summary>
        bool IsHandled { get; set; }
        /// <summary>
        /// Key data
        /// </summary>
        ConsoleKeyInfo KeyInfo { get; }
        /// <summary>
        /// UI key input action
        /// </summary>
        KeyInputType InputType { get; }
    }
}
