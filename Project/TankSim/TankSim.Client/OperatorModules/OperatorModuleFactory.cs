using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TIPC.Core.Collections.Generic;

namespace TankSim.Client.OperatorModules
{
    public class OperatorModuleFactory : IOperatorModuleFactory
    {
        private static readonly ListDictionary<OperatorRoles, Type> _roleMap = new ListDictionary<OperatorRoles, Type>();


        static OperatorModuleFactory()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typeOpMod = typeof(IOperatorModule);
            var qry = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => typeOpMod.IsAssignableFrom(x))
                .Select(x => (type: x, attr: x.GetCustomAttribute<OperatorRoleAttribute>()))
                .Where(x => !(x.attr is null));

            foreach (var (type, attr) in qry)
            {
                foreach (OperatorRoles r in Enum.GetValues(typeof(OperatorRoles)))
                {
                    if ((attr.OpRoles & r) != 0)
                    {
                        _roleMap.Add(r, type);
                    }
                }

            }
        }


        private readonly IServiceProvider _serviceProvider;
        public OperatorModuleFactory(IServiceProvider ServiceProvider)
        {
            _serviceProvider = ServiceProvider;
        }


        public IOperatorModuleCollection GetModuleCollection(OperatorRoles Roles)
        {
            var collection = new OperatorModuleCollection();

            var qry = Enum.GetValues(typeof(OperatorRoles))
                .Cast<OperatorRoles>()
                .Where(role => (Roles & role) != 0)
                .SelectMany(r => _roleMap[r])
                .Where(x => !(x is null))
                .Select(x => ActivatorUtilities.CreateInstance(_serviceProvider, x))
                .Cast<IOperatorModule>();

            collection.AddModules(qry);
            return collection;
        }
    }
}
