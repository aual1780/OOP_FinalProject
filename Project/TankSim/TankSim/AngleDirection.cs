﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim
{
    /// <summary>
    /// Left/Right directions.  Used for turning and gun angle
    /// </summary>
    public enum AngleDirection
    {
        /// <summary>
        /// Stop movement
        /// </summary>
        Stop = 0,
        /// <summary>
        /// Turn left
        /// </summary>
        Left = 1, 
        /// <summary>
        /// Turn right
        /// </summary>
        Right = 2
    }
}