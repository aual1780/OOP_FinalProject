using Microsoft.Extensions.DependencyInjection;
using System;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.OperatorModules
{
    public class OperatorModuleFactory : IOperatorModuleFactory
    {
        readonly IServiceProvider _serviceProvider;

        public OperatorModuleFactory(IServiceProvider ServiceProvider)
        {
            _serviceProvider = ServiceProvider;
        }

        public IOperatorModuleCollection GetModuleCollection(OperatorRoles Roles)
        {
            var collection = new OperatorModuleCollection();
            if ((Roles & OperatorRoles.Driver) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<GuiDriver>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.FireControl) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<GuiFireControl>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.GunLoader) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<GuiGunLoader>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.GunRotation) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<GuiGunRotation>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.Navigator) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<GuiNavigator>(_serviceProvider));
            }
            if ((Roles & OperatorRoles.RangeFinder) != 0)
            {
                collection.AddModule(ActivatorUtilities.CreateInstance<GuiRangeFinder>(_serviceProvider));
            }
            return collection;
        }
    }
}
