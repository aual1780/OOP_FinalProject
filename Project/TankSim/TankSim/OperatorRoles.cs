using System;

namespace TankSim
{
    /// <summary>
    /// Tank operator roles.
    /// Represents the different control sets that users can have
    /// </summary>
    [Flags]
    public enum OperatorRoles
    {
        /// <summary>
        /// No controls
        /// </summary>
        None = 0,
        /// <summary>
        /// Tank driver.  Forward/Backward
        /// </summary>
        Driver = 1 << 0,
        /// <summary>
        /// Tank navigator.  Left/Right steering
        /// </summary>
        Navigator = 1 << 1,
        /// <summary>
        /// Gun angle operator.  Turn the main turret left/right
        /// </summary>
        GunAngle = 1 << 2,
        /// <summary>
        /// Weapon range finder.  Control gun distance
        /// </summary>
        RangeFinder = 1 << 3,
        /// <summary>
        /// Weapon fire control.  Shoot the guns
        /// </summary>
        FireControl = 1 << 4,
        /// <summary>
        /// Gun loader.  Load the gun and change ammo type
        /// </summary>
        Loader = 1 << 5,

    }
}
