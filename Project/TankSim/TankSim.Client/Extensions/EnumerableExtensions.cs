using System.Collections.Generic;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.Extensions
{
    /// <summary>
    /// IEnumerable extensions
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Input extension for <see cref="IOperatorInputModule"/>.  Uses early return semantics based on IsHandled flag
        /// </summary>
        /// <param name="list"></param>
        /// <param name="msg"></param>
        public static void SendInput(this IEnumerable<IOperatorInputModule> list, IOperatorInputMsg msg)
        {
            //Pattern: Chain of Responsibility
            //Pattern: Iterator
            foreach (var itm in list)
            {
                if (msg.IsHandled)
                    return;
                itm.HandleInput(msg);
            }
        }
    }
}
