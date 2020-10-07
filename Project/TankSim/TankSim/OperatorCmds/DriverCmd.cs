using System;
using TIPC.Core.Tools;

namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - driver operation
    /// </summary>
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
        public DriveDirection Direction { get; private set; }

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
