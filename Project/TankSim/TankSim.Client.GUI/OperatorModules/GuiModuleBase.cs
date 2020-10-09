using System;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.OperatorModules
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GuiModuleBase : IOperatorModule
    {
        public abstract void HandleInput(IOperatorInputMsg Input);

        public bool ValidateKeyPress(IOperatorInputMsg Input, string TargetInput)
        {
            var eq1 = string.Equals(TargetInput, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase);
            var eq2 = eq1 || string.Equals(TargetInput, Input.KeyInfo.Key.ToString(), StringComparison.OrdinalIgnoreCase);
            var eq3 = eq2 || string.Equals(TargetInput, Input.KeyInfo.Modifiers.ToString(), StringComparison.OrdinalIgnoreCase);
            return eq3;
        }

        public abstract void Dispose();
    }
}
