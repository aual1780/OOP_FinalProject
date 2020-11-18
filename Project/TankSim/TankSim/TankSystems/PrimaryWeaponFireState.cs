
namespace TankSim.TankSystems
{
    /// <summary>
    /// Calculated primary weapon fire method
    /// </summary>
    public enum PrimaryWeaponFireState
    {
        /// <summary>
        /// Weapon unloaded
        /// </summary>
        Empty = 0,
        /// <summary>
        /// Valid shoot
        /// </summary>
        Valid = 1,
        /// <summary>
        /// Weapon mid-load, explosive misfire
        /// </summary>
        Misfire = 2
    }
}