using System;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Operator role module for user input modules
    /// </summary>
    public interface IOperatorInputModule : IOperatorModule, IDisposable
    {
        /// <summary>
        /// Handle user input
        /// </summary>
        /// <param name="Input"></param>
        void HandleInput(IOperatorInputMsg Input);
    }
}
