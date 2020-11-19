using System;
using MessagePack;
using TIPC.Core.Tools;

namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - driver operation
    /// </summary>
    [MessagePackObject]
    public sealed class DriverCmd
    {
        /// <summary>
        /// Static immutable stop command
        /// </summary>
        public static DriverCmd Stop { get; } = new DriverCmd(DriveDirection.Stop);
        /// <summary>
        /// Static immutable forward command
        /// </summary>
        public static DriverCmd Forward { get; } = new DriverCmd(DriveDirection.Forward);
        /// <summary>
        /// Static immutable backward command
        /// </summary>
        public static DriverCmd Backward { get; } = new DriverCmd(DriveDirection.Backward);


        /// <summary>
        /// Drive direction
        /// </summary>
        [Key(0)]
        public DriveDirection Direction { get; private set; }

        /// <summary>
        /// Command creation time
        /// </summary>
        [Key(1)]
        public DateTime InitTime { get; private set; } = HighResolutionDateTime.UtcNow;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="Direction">Drive direction</param>
        public DriverCmd(DriveDirection Direction)
        {
            this.Direction = Direction;
        }
    }
}
