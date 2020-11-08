using ArdNet;
using ArdNet.Server;
using System;
using System.Collections.Generic;
using TankSim.GameHost.TankSystems;
using TankSim.OperatorCmds;
using TankSim.OperatorDelegates;
using TIPC.Core.Tools;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost
{
    /// <summary>
    /// Operator role facade to encapsulate ArdNet behavior
    /// </summary>
    public class OperatorCmdFacade : IDisposable
    {
        private readonly object _loadLockObj = new object();
        private bool _isLoaded = false;
        private DateTime _lastLoadTime = DateTime.MinValue;
        private readonly List<IDisposable> _proxySet = new List<IDisposable>();



        /// <summary>
        /// Event triggered when tank movement vector is changed
        /// </summary>
        public event TankMovementCmdEventHandler MovementChanged;

        /// <summary>
        /// Event triggered when tank movement vector is changed
        /// </summary>
        public event TankMovementCmdEventHandler AimChanged;

        /// <summary>
        /// Event triggered when driver command is received
        /// </summary>
        public event OperatorCmdEventHandler<DriverCmd> DriverCmdReceived;

        public event Action<IConnectedSystemEndpoint, PrimaryWeaponFiredEventArgs> PrimaryWeaponFired;

        public event Action<IConnectedSystemEndpoint> SecondaryWeaponFired;

        public event Action<IConnectedSystemEndpoint> PrimaryGunLoaded;

        public event Action<IConnectedSystemEndpoint> PrimaryAmmoCycled;

        /// <summary>
        /// Event triggered when gun rotation command is received
        /// </summary>
        public event OperatorCmdEventHandler<GunRotationCmd> GunRotationCmdReceived;

        /// <summary>
        /// Event triggered when navigator command is received
        /// </summary>
        public event OperatorCmdEventHandler<NavigatorCmd> NavigatorCmdReceived;

        /// <summary>
        /// Event triggered when range finder command is received
        /// </summary>
        public event OperatorCmdEventHandler<RangeFinderCmd> RangeFinderCmdReceived;


        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdServer"></param>
        public OperatorCmdFacade(IArdNetServer ArdServer)
        {
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
            {
                var proxy = new DriverDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => DriverCmdReceived(x, y);
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.Driver) != 0;
                });
                _proxySet.Add(proxy);
            }
            {
                var proxy = new FireControlDelegate(ArdServer);
                proxy.CmdReceived += (x, y) =>
                {
                    if (y.WeaponType == FireControlType.Primary)
                    {
                        lock (_loadLockObj)
                        {
                            var now = HighResolutionDateTime.UtcNow;
                            var diff = now - _lastLoadTime;
                            var isValidFire = diff >= Constants.Gameplay.ReloadDuration;
                            var isLoaded = _isLoaded && isValidFire;
                            var isMisfire = _isLoaded && !isValidFire;

                            var arg = new PrimaryWeaponFiredEventArgs(isLoaded, isMisfire);
                            PrimaryWeaponFired?.Invoke(x, arg);

                            _isLoaded = false;
                        }
                    }
                    else if (y.WeaponType == FireControlType.Secondary)
                    {
                        SecondaryWeaponFired?.Invoke(x);
                    }
                };
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.FireControl) != 0;
                });
                _proxySet.Add(proxy);
            }
            {
                var proxy = new GunLoaderDelegate(ArdServer);
                proxy.CmdReceived += (x, y) =>
                {
                    if (y.LoaderType == GunLoaderType.Load)
                    {
                        lock (_loadLockObj)
                        {
                            _isLoaded = true;
                            _lastLoadTime = HighResolutionDateTime.UtcNow;
                            PrimaryGunLoaded?.Invoke(x);
                        }
                    }
                    else if (y.LoaderType == GunLoaderType.CycleAmmoType)
                    {
                        PrimaryAmmoCycled?.Invoke(x);
                    }
                };
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.GunLoader) != 0;
                });
                _proxySet.Add(proxy);
            }
            {
                var proxy = new GunRotationDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => GunRotationCmdReceived(x, y);
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.GunRotation) != 0;
                });
                _proxySet.Add(proxy);
            }
            {
                var proxy = new NavigatorDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => NavigatorCmdReceived(x, y);
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.Navigator) != 0;
                });
                _proxySet.Add(proxy);
            }
            {
                var proxy = new RangeFinderDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => RangeFinderCmdReceived(x, y);
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.RangeFinder) != 0;
                });
                _proxySet.Add(proxy);
            }
        }


        /// <summary>
        /// Release proxy hooks
        /// </summary>
        public void Dispose()
        {
            _proxySet.DisposeAll();
        }
    }
}
