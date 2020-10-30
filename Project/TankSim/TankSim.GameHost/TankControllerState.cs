namespace TankSim.GameHost
{
    /// <summary>
    /// Tank controller system state object
    /// </summary>
    public class TankControllerState
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Roles
        /// </summary>
        public OperatorRoles Roles { get; set; }
        /// <summary>
        /// IsReady
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// Create new instance
        /// </summary>
        public TankControllerState()
        {
        }
    }
}
