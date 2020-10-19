using ArdNet.Server;
using System;
using System.Collections.Generic;
using TankSim.OperatorDelegates;
using TIPC.Core.Tools.Extensions.IEnumerable;

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
        /// Event triggered when driver command is received
        /// </summary>
        public event DriverCmdEventHandler DriverCmdReceived;

        /// <summary>
        /// Event triggered when fire control command is received
        /// </summary>
        public event FireControlCmdEventHandler FireControlCmdReceived;

        /// <summary>
        /// Event triggered when gun loader command is received
        /// </summary>
        public event GunLoaderCmdEventHandler GunLoaderCmdReceived;

        /// <summary>
        /// Event triggered when gun rotation command is received
        /// </summary>
        public event GunRotationCmdEventHandler GunRotationCmdReceived;

        /// <summary>
        /// Event triggered when navigator command is received
        /// </summary>
        public event NavigatorCmdEventHandler NavigatorCmdReceived;

        /// <summary>
        /// Event triggered when range finder command is received
        /// </summary>
        public event RangeFinderCmdEventHandler RangeFinderCmdReceived;


        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="ArdServer"></param>
        public OperatorCmdFacade(IArdNetServer ArdServer)
        {
            {
                var proxy = new TankMovementDelegate(ArdServer);
                proxy.MovementChanged += (x, y) => MovementChanged(x, y);
                _proxySet.Add(proxy);
            }
            {
                var proxy = new DriverDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => DriverCmdReceived(x, y);
                _proxySet.Add(proxy);
            }
            {
                var proxy = new FireControlDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => FireControlCmdReceived(x, y);
                _proxySet.Add(proxy);
            }
            {
                var proxy = new GunLoaderDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => GunLoaderCmdReceived(x, y);
                _proxySet.Add(proxy);
            }
            {
                var proxy = new GunRotationDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => GunRotationCmdReceived(x, y);
                _proxySet.Add(proxy);
            }
            {
                var proxy = new NavigatorDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => NavigatorCmdReceived(x, y);
                _proxySet.Add(proxy);
            }
            {
                var proxy = new RangeFinderDelegate(ArdServer);
                proxy.CmdReceived += (x, y) => RangeFinderCmdReceived(x, y);
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
