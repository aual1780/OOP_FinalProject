using System;
using System.Collections.Generic;
using System.Text;

namespace TankSim
{
    /// <summary>
    /// Gun loader operation type
    /// </summary>
    public enum GunLoaderType
    {
        /// <summary>
        /// Load primary weapon
        /// </summary>
        Load = 1,
        /// <summary>
        /// Change the primary weapon ammo type
        /// </summary>
        CycleAmmoType = 2
    }
}
