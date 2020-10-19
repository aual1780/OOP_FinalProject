using System;

namespace TankSim
{
    /// <summary>
    /// Attribute to define operator role mappings for operator modules.
    /// Used for dynamic factory loading
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OperatorRoleAttribute : Attribute
    {
        /// <summary>
        /// Associated perator roles
        /// </summary>
        public OperatorRoles OpRoles { get; private set; }

        /// <summary>
        /// Create isntance
        /// </summary>
        /// <param name="OpRoles"></param>
        public OperatorRoleAttribute(OperatorRoles OpRoles)
        {
            this.OpRoles = OpRoles;
        }
    }
}
