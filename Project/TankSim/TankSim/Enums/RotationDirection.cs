namespace TankSim
{
    /// <summary>
    /// Left/Right directions.  Used for turning and gun angle
    /// </summary>
    public enum RotationDirection
    {
        /// <summary>
        /// Stop movement
        /// </summary>
        Stop = 0,
        /// <summary>
        /// Turn left
        /// </summary>
        Left = MovementDirection.West,
        /// <summary>
        /// Turn right
        /// </summary>
        Right = MovementDirection.East
    }
}
