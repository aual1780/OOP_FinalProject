using Microsoft.Extensions.DependencyInjection;
using System;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.OperatorModules
{
    public class OperatorModuleFactory
    {
        readonly IServiceProvider _serviceProvider;

        public OperatorModuleFactory(IServiceProvider ServiceProvider)
        {
            _serviceProvider = ServiceProvider;
        }

        public IOperatorModuleCollection GetModuleCollection(OperatorRoles Roles)
        {
            //TODO: add all modules
            var collection = new OperatorModuleCollection();
            if((Roles & OperatorRoles.Driver) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<CliDriver>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.FireControl) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<CliFireControl>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.GunLoader) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<CliGunLoader>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.GunRotation) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<CliGunRotation>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.Navigator) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<CliNavigator>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.RangeFinder) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<CliRangeFinder>(_serviceProvider));
            }
            return collection;
        }
    }
}
