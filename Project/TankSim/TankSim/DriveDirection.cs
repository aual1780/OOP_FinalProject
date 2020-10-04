using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim
{
    /// <summary>
    /// Front/Back direction.  Used for driving and range finding
    /// </summary>
    public enum DriveDirection
    {
        /// <summary>
        /// Stop movement
        /// </summary>
        Stop = 0,
        /// <summary>
        /// Forward command
        /// </summary>
        Forward = 1,
        /// <summary>
        /// Backward command
        /// </summary>
        Backward = 2
    }
}
