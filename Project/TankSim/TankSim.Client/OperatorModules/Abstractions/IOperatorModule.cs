using System;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Operator role module
    /// </summary>
    public interface IOperatorModule : IDisposable
    {
        /// <summary>
        /// Handle user input
        /// </summary>
        /// <param name="Input"></param>
        void HandleInput(IOperatorInputMsg Input);
    }
}
