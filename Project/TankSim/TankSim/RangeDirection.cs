using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim
{
    /// <summary>
    /// Range finder targeting direction
    /// </summary>
    public enum RangeDirection
    {
        /// <summary>
        /// Stop movement
        /// </summary>
        Stop = 0,
        /// <summary>
        /// Aim farther away
        /// </summary>
        Farther = 1,
        /// <summary>
        /// Aim closer
        /// </summary>
        Closer = 2
    }
}
