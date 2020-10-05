using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TankSim.Client.Config;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.OperatorModules
{
    public class OperatorModuleFactory
    {
        readonly IArdNetClient _ardClient;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public OperatorModuleFactory(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            _ardClient = (ArdClient ?? throw new ArgumentNullException(nameof(ArdClient)));
            _keyBinding = KeyBinding;
        }

        public IOperatorModuleCollection GetModuleCollection(OperatorRoles Roles)
        {
            //TODO: add all modules
            var collection = new OperatorModuleCollection();
            if((Roles & OperatorRoles.Driver) != 0)
            {
                collection.AddModule(new CliDriver(_ardClient, _keyBinding));
            }
            return collection;
        }
    }
}
