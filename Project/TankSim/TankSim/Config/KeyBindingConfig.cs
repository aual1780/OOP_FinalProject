namespace TankSim.Config
{
    //TODO
    /// <summary>
    /// Key binding configuration
    /// </summary>
    public class KeyBindingConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public class DriverConfig
        {
            /// <summary>
            /// Drive forward
            /// </summary>
            public string Forward { get; private set; }
            /// <summary>
            /// Drive backward
            /// </summary>
            public string Backward { get; private set; }
        }

        /// <summary>
        /// Driver key bindings
        /// </summary>
        public DriverConfig Driver { get; private set; }

    }
}
