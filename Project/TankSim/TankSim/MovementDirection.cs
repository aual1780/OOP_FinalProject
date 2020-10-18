using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim
{
    /// <summary>
    /// Actual tank movement direction accounting for both driver and navigator vectors
    /// </summary>
    [Flags]
    public enum MovementDirection
    {
        /// <summary>
        /// No movement
        /// </summary>
        Stop = 0,
        /// <summary>
        /// West vector applied
        /// </summary>
        West = 1 << 0,
        /// <summary>
        /// North vector applied
        /// </summary>
        North = 1 << 1,
        /// <summary>
        /// East vector applied
        /// </summary>
        East = 1 << 2,
        /// <summary>
        /// South vector applied
        /// </summary>
        South = 1 << 3
    }
}
