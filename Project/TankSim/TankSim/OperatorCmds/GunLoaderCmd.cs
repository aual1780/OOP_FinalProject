namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - fire control operation
    /// </summary>
    public sealed class GunLoaderCmd
    {
        /// <summary>
        /// Static immutable load primary weapon command
        /// </summary>
        public static GunLoaderCmd Load { get; } = new GunLoaderCmd(GunLoaderType.Load);
        /// <summary>
        /// Static immutable cycle ammo type command
        /// </summary>
        public static GunLoaderCmd CycleAmmoType { get; } = new GunLoaderCmd(GunLoaderType.CycleAmmoType);


        /// <summary>
        /// Loader type
        /// </summary>
        public GunLoaderType LoaderType { get; private set; }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="LoaderType">Gun loader operation type</param>
        public GunLoaderCmd(GunLoaderType LoaderType)
        {
            this.LoaderType = LoaderType;
        }
    }
}
