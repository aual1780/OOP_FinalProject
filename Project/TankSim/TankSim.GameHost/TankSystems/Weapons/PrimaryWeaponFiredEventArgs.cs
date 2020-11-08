using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim.GameHost.TankSystems
{
    /// <summary>
    /// Event arg for primary weapons
    /// </summary>
    public class PrimaryWeaponFiredEventArgs
    {
        /// <summary>
        /// Indicates that the gun was not loaded when the fire command was sent
        /// </summary>
        public bool IsLoaded { get; }

        /// <summary>
        /// Indicates that the gun was midload then the fire command was sent
        /// </summary>
        public bool IsMisfire { get; }

        /// <summary>
        /// Indicates whether or not the gun should fire
        /// </summary>
        public bool ShouldShoot { get; }

        public PrimaryWeaponFiredEventArgs(bool IsLoaded, bool IsMisfire)
        {
            this.IsLoaded = IsLoaded;
            this.IsMisfire = IsMisfire;
            this.ShouldShoot = IsLoaded && !IsMisfire;
        }
    }
}
