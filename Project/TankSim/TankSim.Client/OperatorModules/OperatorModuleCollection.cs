using System;
using System.Collections.Generic;
using TankSim.Client.OperatorModules;
using TIPC.Core.Tools.Extensions.IEnumerable;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Operator module collection
    /// </summary>
    public class OperatorModuleCollection : IOperatorModuleCollection
    {
        private readonly List<IOperatorModule> _modules = new List<IOperatorModule>();

        /// <summary>
        /// 
        /// </summary>
        public OperatorModuleCollection()
        {

        }

        /// <summary>
        /// Add new operator module
        /// </summary>
        /// <param name="Module"></param>
        public void AddModule(IOperatorModule Module)
        {
            if (Module is null)
            {
                throw new ArgumentNullException(nameof(Module));
            }

            _modules.Add(Module);
        }

        /// <summary>
        /// Add new operator module
        /// </summary>
        /// <param name="Modules"></param>
        public void AddModules(IEnumerable<IOperatorModule> Modules)
        {
            if (Modules is null)
            {
                throw new ArgumentNullException(nameof(Modules));
            }
            foreach (var mod in Modules)
            {
                if (mod != null)
                {
                    _modules.Add(mod);
                }
            }
        }

        /// <summary>
        /// Send input to all managed modules.
        /// Propagation stops after the input command is handled
        /// </summary>
        /// <param name="Input"></param>
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

        /// <summary>
        /// Dispose all operator modules
        /// </summary>
        public void Dispose()
        {
            _modules.DisposeAll();
        }
    }
}
