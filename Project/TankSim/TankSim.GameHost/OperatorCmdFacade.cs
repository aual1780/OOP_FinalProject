using ArdNet;
using ArdNet.Server;
using System;
using System.Collections.Generic;
using TankSim.GameHost.TankSystems;
using TankSim.OperatorCmds;
using TankSim.OperatorDelegates;
using TankSim.TankSystems;
using TIPC.Core.Tools;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost
{
    /// <summary>
    /// Operator role facade to encapsulate ArdNet behavior
    /// </summary>
    public class OperatorCmdFacade : IDisposable
    {
        private readonly List<IDisposable> _proxySet = new();

        /// <summary>
        /// Event triggered when tank movement vector is changed
        /// </summary>
        public event TankMovementCmdEventHandler MovementChanged;

        /// <summary>
        /// Event triggered when tank movement vector is changed
        /// </summary>
        public event TankMovementCmdEventHandler AimChanged;

        /// <summary>
        /// Event triggered when main gun is fired.
        /// Arg indicates the state that the weapon was in at the moment of firing
        /// </summary>
        public event Action<IConnectedSystemEndpoint, PrimaryWeaponFireState> PrimaryWeaponFired;

        /// <summary>
        /// Event triggered when secondary gun is fired
        /// </summary>
        public event Action<IConnectedSystemEndpoint> SecondaryWeaponFired;

        /// <summary>
        /// Event triggered when main gun is loaded
        /// </summary>
        public event Action<IConnectedSystemEndpoint> PrimaryGunLoaded;

        /// <summary>
        /// Event triggered when primary ammo type is cycled
        /// </summary>
        public event Action<IConnectedSystemEndpoint> PrimaryAmmoCycled;

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdServer"></param>
        public OperatorCmdFacade(IArdNetServer ArdServer)
        {
            RegisterTankMovement(ArdServer);
            RegisterTankAiming(ArdServer);
            RegisterTankWeapons(ArdServer);
        }


        private void RegisterTankMovement(IArdNetServer ArdServer)
        {
            var proxy = new TankMovementDelegate(ArdServer);
            proxy.MovementChanged += (x, y) => MovementChanged(x, y);
            proxy.Validator.AddFilter(e =>
            {
                var state = (TankControllerState)e.Endpt.UserState;
                var roles = OperatorRoles.Driver | OperatorRoles.Navigator;
                var ns = MovementDirection.North | MovementDirection.South;
                var ew = MovementDirection.East | MovementDirection.West;
                if ((state.Roles & roles) == roles)
                {
                    return true;
                }
                else if ((state.Roles & OperatorRoles.Driver) != 0)
                {
                    if ((e.Dir & ew) == 0)
                        return true;
                }
                else if ((state.Roles & OperatorRoles.Navigator) != 0)
                {
                    if ((e.Dir & ns) == 0)
                        return true;
                }
                return false;
            });
            _proxySet.Add(proxy);
        }

        private void RegisterTankAiming(IArdNetServer ArdServer)
        {
            var proxy = new TankAimingDelegate(ArdServer);
            proxy.AimChanged += (x, y) => AimChanged(x, y);
            proxy.Validator.AddFilter(e =>
            {
                var state = (TankControllerState)e.Endpt.UserState;
                var roles = OperatorRoles.RangeFinder | OperatorRoles.GunRotation;
                var ns = MovementDirection.North | MovementDirection.South;
                var ew = MovementDirection.East | MovementDirection.West;
                if ((state.Roles & roles) == roles)
                {
                    return true;
                }
                else if ((state.Roles & OperatorRoles.RangeFinder) != 0)
                {
                    if ((e.Dir & ew) == 0)
                        return true;
                }
                else if ((state.Roles & OperatorRoles.GunRotation) != 0)
                {
                    if ((e.Dir & ns) == 0)
                        return true;
                }
                return false;
            });
            _proxySet.Add(proxy);
        }

        private void RegisterTankWeapons(IArdNetServer ArdServer)
        {
            var fireProxy = new FireControlDelegate(ArdServer);
            fireProxy.Validator.AddFilter(x =>
            {
                var state = (TankControllerState)x.SourceEndpoint.UserState;
                return (state.Roles & OperatorRoles.FireControl) != 0;
            });
            _proxySet.Add(fireProxy);

            var loadProxy = new GunLoaderDelegate(ArdServer);
            loadProxy.Validator.AddFilter(x =>
            {
                var state = (TankControllerState)x.SourceEndpoint.UserState;
                return (state.Roles & OperatorRoles.GunLoader) != 0;
            });
            _proxySet.Add(loadProxy);

            var weaponProxy = new TankWeaponDelegate(fireProxy, loadProxy);
            weaponProxy.PrimaryWeaponFired += (s, e) => PrimaryWeaponFired?.Invoke(s, e);
            weaponProxy.SecondaryWeaponFired += (s) => SecondaryWeaponFired?.Invoke(s);
            weaponProxy.PrimaryGunLoaded += (s) => PrimaryGunLoaded?.Invoke(s);
            weaponProxy.PrimaryAmmoCycled += (s) => PrimaryAmmoCycled?.Invoke(s);
            _proxySet.Add(weaponProxy);
        }


        /// <summary>
        /// Release proxy hooks
        /// </summary>
        public void Dispose()
        {
            _proxySet.DisposeAll();
            GC.SuppressFinalize(this);
        }
    }
}
