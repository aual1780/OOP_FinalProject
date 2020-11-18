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
        Forward = MovementDirection.North,
        /// <summary>
        /// Backward command
        /// </summary>
        Backward = MovementDirection.South
    }
}
