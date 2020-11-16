using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TIPC.Core.Collections.Generic;
using TIPC.Core.Tools;

namespace TankSim.Client.OperatorModules
{
    /// <summary>
    /// Magic factory for loading operator modules from execution context at runtime.
    /// Searches execution process for <see cref="IOperatorModule"/> implementations with <see cref="OperatorRoleAttribute"/> defined.
    /// Maps operator role to tagged modules.
    /// Instantiates modules on-demand when a role is requested.
    /// </summary>
    public class OperatorModuleFactory<T> : IOperatorModuleFactory<T>, IOperatorModuleFactory
        where T : IOperatorModule
    {
        private static readonly object _startupLock = new();
        private static readonly ListDictionary<OperatorRoles, HashSet<Type>, Type> _roleMap;

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
                _roleMap = new ListDictionary<OperatorRoles, HashSet<Type>, Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var typeOpMod = typeof(T);
                var qry = assemblies
                    .SelectMany(x => x.GetTypes())
                    .Where(x => (x.Attributes & TypeAttributes.Abstract) == 0)
                    .Where(x => (x.Attributes & TypeAttributes.Interface) == 0)
                    .Where(x => typeOpMod.IsAssignableFrom(x))
                    .Select(x => (type: x, attrs: x.GetCustomAttributes<OperatorRoleAttribute>().ToList()))
                    .Where(x => !(x.attrs is null) && x.attrs.Count > 0);

                foreach (var (type, attrs) in qry)
                {
                    var roles =
                        attrs
                        .SelectMany(x => EnumTools.GetSelectedFlags(x.OpRoles))
                        .Distinct();

                    foreach (var r in roles)
                    {
                        _roleMap.Add(r, type);
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


        IEnumerable<IOperatorModule> IOperatorModuleFactory.GetModuleCollection(OperatorRoles Roles) => GetModuleCollection(Roles).Cast<IOperatorModule>();

        /// <summary>
        /// Get module collection based on requested operator roles.
        /// </summary>
        /// <param name="Roles">Set of required operator roles to load</param>
        /// <returns>Return a module collection loaded with modules for all requested operator roles</returns>
        public IEnumerable<T> GetModuleCollection(OperatorRoles Roles)
        {
            var collection = new List<T>();

            var qry = EnumTools.GetSelectedFlags(Roles)
                .SelectMany(r => _roleMap[r])
                .Where(x => !(x is null))
                .Distinct()
                .Select(x => ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, x))
                .OfType<T>();

            collection.AddRange(qry);
            return collection;
        }
    }
}
