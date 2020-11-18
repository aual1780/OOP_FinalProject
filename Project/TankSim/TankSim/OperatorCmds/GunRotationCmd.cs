using System;
using TIPC.Core.Tools;

namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - gun rotation operation
    /// </summary>
    public sealed class GunRotationCmd
    {
        /// <summary>
        /// Static immutable stop command
        /// </summary>
        public static GunRotationCmd Stop { get; } = new GunRotationCmd(RotationDirection.Stop);
        /// <summary>
        /// Static immutable left command
        /// </summary>
        public static GunRotationCmd Left { get; } = new GunRotationCmd(RotationDirection.Left);
        /// <summary>
        /// Static immutable right command
        /// </summary>
        public static GunRotationCmd Right { get; } = new GunRotationCmd(RotationDirection.Right);


        /// <summary>
        /// Rotation direction
        /// </summary>
        public RotationDirection Direction { get; private set; }

        /// <summary>
        /// Command creation time
        /// </summary>
        public DateTime InitTime { get; private set; } = HighResolutionDateTime.UtcNow;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="Direction">Rotation direction</param>
        public GunRotationCmd(RotationDirection Direction)
        {
            this.Direction = Direction;
        }
    }
}
