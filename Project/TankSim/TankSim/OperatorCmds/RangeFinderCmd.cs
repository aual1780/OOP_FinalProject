﻿using System;
using MessagePack;
using TIPC.Core.Tools;

namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - range finder operation
    /// </summary>
    [MessagePackObject]
    public sealed class RangeFinderCmd
    {
        /// <summary>
        /// Static immutable stop command
        /// </summary>
        public static RangeFinderCmd Stop { get; } = new RangeFinderCmd(RangeDirection.Stop);
        /// <summary>
        /// Static immutable aim farther command
        /// </summary>
        public static RangeFinderCmd Farther { get; } = new RangeFinderCmd(RangeDirection.Farther);
        /// <summary>
        /// Static immutable aim closer command
        /// </summary>
        public static RangeFinderCmd Closer { get; } = new RangeFinderCmd(RangeDirection.Closer);


        /// <summary>
        /// Range direction
        /// </summary>
        [Key(0)]
        public RangeDirection Direction { get; private set; }

        /// <summary>
        /// Command creation time
        /// </summary>
        [Key(1)]
        public DateTime InitTime { get; private set; } = HighResolutionDateTime.UtcNow;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="Direction">Range direction</param>
        public RangeFinderCmd(RangeDirection Direction)
        {
            this.Direction = Direction;
        }
    }
}
