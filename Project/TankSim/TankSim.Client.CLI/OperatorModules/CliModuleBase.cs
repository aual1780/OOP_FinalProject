using System;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.OperatorModules
{
    public abstract class CliModuleBase : IOperatorModule
    {
        public abstract void Dispose();

        public abstract void HandleInput(IOperatorInputMsg Input);

        protected bool ValidateKeyPress(IOperatorInputMsg Input, string TargetInput)
        {
            var eq1 = string.Equals(TargetInput, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase);
            var eq2 = eq1 || string.Equals(TargetInput, Input.KeyInfo.Key.ToString(), StringComparison.OrdinalIgnoreCase);
            return eq2;
        }
    }
}
