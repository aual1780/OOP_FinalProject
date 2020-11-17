using System;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Shared base for operator modules
    /// </summary>
    public abstract class OperatorInputModuleBase : IOperatorInputModule
    {
        /// <summary>
        /// Handle input key sequence
        /// </summary>
        /// <param name="Input"></param>
        public abstract void HandleInput(IOperatorInputMsg Input);

        /// <summary>
        /// Validate input keycode against target key sequence
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="TargetInput"></param>
        /// <returns></returns>
        public static bool ValidateKeyPress(IOperatorInputMsg Input, string TargetInput)
        {
            var eq1 = string.Equals(TargetInput, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase);
            var eq2 = eq1 || string.Equals(TargetInput, Input.KeyInfo.Key.ToString(), StringComparison.OrdinalIgnoreCase);
            var eq3 = eq2 || string.Equals(TargetInput, Input.KeyInfo.Modifiers.ToString(), StringComparison.OrdinalIgnoreCase);
            return eq3;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public abstract void Dispose();
    }
}
