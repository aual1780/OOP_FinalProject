using ArdNet.Server;
using System;
using System.Collections.Generic;
using TankSim.OperatorCmds;
using TankSim.OperatorDelegates;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost
{
    /// <summary>
    /// Operator role facade to encapsulate ArdNet behavior
    /// </summary>
    public class OperatorCmdFacade : IDisposable
    {
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

        /// <summary>
        /// Event triggered when fire control command is received
        /// </summary>
        public event OperatorCmdEventHandler<FireControlCmd> FireControlCmdReceived;

        /// <summary>
        /// Event triggered when gun loader command is received
        /// </summary>
        public event OperatorCmdEventHandler<GunLoaderCmd> GunLoaderCmdReceived;

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
                proxy.CmdReceived += (x, y) => FireControlCmdReceived(x, y);
                proxy.Validator.AddFilter(x =>
                {
                    var state = (TankControllerState)x.SourceEndpoint.UserState;
                    return (state.Roles & OperatorRoles.FireControl) != 0;
                });
                _proxySet.Add(proxy);
            }
            {
                var proxy = new GunLoaderDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => GunLoaderCmdReceived(x, y);
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
