using System;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Collection of operator modules
    /// </summary>
    public interface IOperatorModuleCollection : IDisposable
    {
        /// <summary>
        /// Send user input to modules
        /// </summary>
        /// <param name="Input"></param>
        void SendInput(IOperatorInputMsg Input);
    }
}
