namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - navigator operation
    /// </summary>
    public class NavigatorCmd
    {
        /// <summary>
        /// Static immutable stop command
        /// </summary>
        public static NavigatorCmd Stop { get; } = new NavigatorCmd(AngleDirection.Stop);
        /// <summary>
        /// Static immutable forward command
        /// </summary>
        public static NavigatorCmd Left { get; } = new NavigatorCmd(AngleDirection.Left);
        /// <summary>
        /// Static immutable backward command
        /// </summary>
        public static NavigatorCmd Right { get; } = new NavigatorCmd(AngleDirection.Right);


        /// <summary>
        /// Drive direction
        /// </summary>
        public AngleDirection Direction { get; private set; }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="Direction">Drive direction</param>
        public NavigatorCmd(AngleDirection Direction)
        {
            this.Direction = Direction;
        }
    }
}
