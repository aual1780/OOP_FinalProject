namespace TankSim.OperatorCmds
{
    /// <summary>
    /// Channel command - fire control operation
    /// </summary>
    public sealed class FireControlCmd
    {
        /// <summary>
        /// Static immutable primary weapon command
        /// </summary>
        public static FireControlCmd Primary { get; } = new FireControlCmd(FireControlType.Primary);
        /// <summary>
        /// Static immutable secondary weapon command
        /// </summary>
        public static FireControlCmd Secondary { get; } = new FireControlCmd(FireControlType.Secondary);


        /// <summary>
        /// Weapon type
        /// </summary>
        public FireControlType WeaponType { get; private set; }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="WeaponType">Weapon type</param>
        public FireControlCmd(FireControlType WeaponType)
        {
            this.WeaponType = WeaponType;
        }
    }
}
