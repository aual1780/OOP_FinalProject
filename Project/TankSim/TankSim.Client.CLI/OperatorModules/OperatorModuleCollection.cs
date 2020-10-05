using System;
using System.Collections.Generic;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.OperatorModules
{
    public class OperatorModuleCollection : IOperatorModuleCollection
    {
        private readonly List<IOperatorModule> _modules = new List<IOperatorModule>();

        public OperatorModuleCollection()
        {

        }

        public void AddModule(IOperatorModule Module)
        {
            if (Module is null)
            {
                throw new ArgumentNullException(nameof(Module));
            }

            _modules.Add(Module);
        }

        public void SendInput(IOperatorInputMsg Input)
        {
            foreach (var module in _modules)
            {
                module.HandleInput(Input);
                if (Input.IsHandled)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            foreach (var module in _modules)
            {
                try
                {
                    module.Dispose();
                }
                catch
                {
                    //noop
                }

            }
        }
    }
}
