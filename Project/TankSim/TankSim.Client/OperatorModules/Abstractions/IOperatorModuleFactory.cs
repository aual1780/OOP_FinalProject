namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Factory to get operator role modules once the client knows its responsibilities
    /// </summary>
    public interface IOperatorModuleFactory
    {
        /// <summary>
        /// Get collection of UI operator modules for the required role sets
        /// </summary>
        /// <param name="Roles"></param>
        /// <returns></returns>
        IOperatorModuleCollection GetModuleCollection(OperatorRoles Roles);
    }
}
