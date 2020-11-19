using System;
using MessagePack;
using TIPC.Core.Tools;

namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - navigator operation
    /// </summary>
    [MessagePackObject]
    public sealed class NavigatorCmd
    {
        /// <summary>
        /// Static immutable stop command
        /// </summary>
        public static NavigatorCmd Stop { get; } = new NavigatorCmd(RotationDirection.Stop);
        /// <summary>
        /// Static immutable left command
        /// </summary>
        public static NavigatorCmd Left { get; } = new NavigatorCmd(RotationDirection.Left);
        /// <summary>
        /// Static immutable right command
        /// </summary>
        public static NavigatorCmd Right { get; } = new NavigatorCmd(RotationDirection.Right);


        /// <summary>
        /// Rotation direction
        /// </summary>
        [Key(0)]
        public RotationDirection Direction { get; private set; }

        /// <summary>
        /// Command creation time
        /// </summary>
        [Key(1)]
        public DateTime InitTime { get; private set; } = HighResolutionDateTime.UtcNow;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="Direction">Rotation direction</param>
        public NavigatorCmd(RotationDirection Direction)
        {
            this.Direction = Direction;
        }
    }
}
