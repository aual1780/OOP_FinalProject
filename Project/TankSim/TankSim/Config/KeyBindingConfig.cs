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
        /// 
        /// </summary>
        public class NavigatorConfig
        {
            /// <summary>
            /// Turn left
            /// </summary>
            public string Left { get; private set; }
            /// <summary>
            /// Turn right
            /// </summary>
            public string Right { get; private set; }
        }

        /// <summary>
        /// Driver key bindings
        /// </summary>
        public DriverConfig Driver { get; private set; }

        /// <summary>
        /// Navigator key bindings
        /// </summary>
        public NavigatorConfig Navigator { get; private set; }

    }
}
