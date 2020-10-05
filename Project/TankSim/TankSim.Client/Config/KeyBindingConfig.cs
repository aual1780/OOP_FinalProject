 using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim.Client.Config
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
            public string Forward { get; set; }
            /// <summary>
            /// Drive backward
            /// </summary>
            public string Backward { get; set; }
        }

        /// <summary>
        /// Driver key bindings
        /// </summary>
        public DriverConfig Driver { get; set; }

    }
}
