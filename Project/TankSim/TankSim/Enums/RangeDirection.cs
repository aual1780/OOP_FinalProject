namespace TankSim
{
    /// <summary>
    /// Range finder targeting direction
    /// </summary>
    public enum RangeDirection
    {
        /// <summary>
        /// Stop movement
        /// </summary>
        Stop = 0,
        /// <summary>
        /// Aim farther away
        /// </summary>
        Farther = MovementDirection.North,
        /// <summary>
        /// Aim closer
        /// </summary>
        Closer = MovementDirection.South
    }
}
