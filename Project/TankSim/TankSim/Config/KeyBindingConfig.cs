namespace TankSim.Config
{
    //TODO
    /// <summary>
    /// Key binding configuration
    /// </summary>
    public class KeyBindingConfig
    {
        /// <summary>
        /// Driver key bindings
        /// </summary>
        public DriverConfig Driver { get; private set; }

        /// <summary>
        /// Fire control key bindings
        /// </summary>
        public FireControlConfig FireControl { get; private set; }

        /// <summary>
        /// Gun loader key bindings
        /// </summary>
        public GunLoaderConfig GunLoader { get; private set; }

        /// <summary>
        /// Gun rotation key bindings
        /// </summary>
        public GunRotationConfig GunRotation { get; private set; }

        /// <summary>
        /// Navigator key bindings
        /// </summary>
        public NavigatorConfig Navigator { get; private set; }

        /// <summary>
        /// Range finder key bindings
        /// </summary>
        public RangeFinderConfig RangeFinder { get; private set; }



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
        public class FireControlConfig
        {
            /// <summary>
            /// Fire primary
            /// </summary>
            public string Primary { get; private set; }
            /// <summary>
            /// Fire secondary
            /// </summary>
            public string Secondary { get; private set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GunLoaderConfig
        {
            /// <summary>
            /// Load primary gun
            /// </summary>
            public string Load { get; private set; }
            /// <summary>
            /// Cycle ammo type
            /// </summary>
            public string CycleAmmo { get; private set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GunRotationConfig
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
        /// 
        /// </summary>
        public class RangeFinderConfig
        {
            /// <summary>
            /// Aim farther
            /// </summary>
            public string Farther { get; private set; }
            /// <summary>
            /// Aim closer
            /// </summary>
            public string Closer { get; private set; }
        }
    }
}
