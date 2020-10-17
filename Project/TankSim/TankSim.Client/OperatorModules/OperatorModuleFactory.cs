using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TIPC.Core.Collections.Generic;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Magic factory for loading operator modules from execution context at runtime.
    /// Searches execution process for <see cref="IOperatorModule"/> implementations with <see cref="OperatorRoleAttribute"/> defined.
    /// Maps operator role to tagged modules.
    /// Instantiates modules on-demand when a role is requested.
    /// </summary>
    public class OperatorModuleFactory : IOperatorModuleFactory
    {
        private static readonly object _startupLock = new object();
        private static readonly ListDictionary<OperatorRoles, Type> _roleMap;

        /// <summary>
        /// Load role->module map at startup
        /// </summary>
        static OperatorModuleFactory()
        {
            //use double-check lock for startup threadsafety
            if (!(_roleMap is null))
            {
                return;
            }

            lock (_startupLock)
            {
                if (!(_roleMap is null))
                {
                    return;
                }
                _roleMap = new ListDictionary<OperatorRoles, Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var typeOpMod = typeof(IOperatorModule);
                var qry = assemblies
                    .SelectMany(x => x.GetTypes())
                    .Where(x => (x.Attributes & TypeAttributes.Abstract) == 0)
                    .Where(x => (x.Attributes & TypeAttributes.Interface) == 0)
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
        }


        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="ServiceProvider"></param>
        public OperatorModuleFactory(IServiceProvider ServiceProvider)
        {
            _serviceProvider = ServiceProvider;
        }


        /// <summary>
        /// Get module collection based on requested operator roles.
        /// </summary>
        /// <param name="Roles">Set of required operator roles to load</param>
        /// <returns>Return a module collection loaded with modules for all requested operator roles</returns>
        public IOperatorModuleCollection GetModuleCollection(OperatorRoles Roles)
        {
            var collection = new OperatorModuleCollection();

            var qry = Enum.GetValues(typeof(OperatorRoles))
                .Cast<OperatorRoles>()
                .Where(role => (Roles & role) != 0)
                .SelectMany(r => _roleMap[r])
                .Where(x => !(x is null))
                .Select(x => ActivatorUtilities.CreateInstance(_serviceProvider, x))
                .OfType<IOperatorModule>();

            collection.AddModules(qry);
            return collection;
        }
    }
}
