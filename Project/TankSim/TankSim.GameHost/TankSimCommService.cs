using ArdNet;
using ArdNet.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TankSim.GameHost.Extensions;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost
{
    /// <summary>
    /// Manage ArdServer communications and messaging
    /// </summary>
    /// <remarks>
    /// Pattern: Composite
    /// </remarks>
    public class TankSimCommService : IDisposable
    {
        readonly ConcurrentDictionary<IPEndPoint, IConnectedSystemEndpoint> _connectedSystems = new();
        readonly Dictionary<string, OperatorRoles> _opRoleCache = new();
        private readonly object _roleLock = new();
        private readonly List<OperatorRoles> _roleSets;
        private readonly CountdownEvent _playerWaiter;
        private int _playerCountCurrent;

        /// <summary>
        /// Underlying ardserver instance
        /// </summary>
        public IArdNetServer ArdServer { get; }
        /// <summary>
        /// Game ID code
        /// </summary>
        public string GameID { get; private set; }
        /// <summary>
        /// Cmd hook interface
        /// </summary>
        public OperatorCmdFacade CmdFacade { get; }
        /// <summary>
        /// Game full lobby player count
        /// </summary>
        public int PlayerCountTarget { get; }
        /// <summary>
        /// Count of players in lobby
        /// </summary>
        public int PlayerCountCurrent => _playerCountCurrent;
        /// <summary>
        /// State objects for all connected controllers
        /// </summary>
        public IEnumerable<TankControllerState> ConnectedControllers => _connectedSystems.Values.Select(x => x.UserState).OfType<TankControllerState>();

        /// <summary>
        /// Event triggered when a controller connects and is set to the ready state (ie triggered when roles and name are set)
        /// </summary>
        public event EventHandler<TankControllerState> TankControllerReady;

        /// <summary>
        /// Create service
        /// </summary>
        /// <param name="ArdServer"></param>
        /// <param name="PlayerCount"></param>
        /// <returns></returns>
        public static async Task<TankSimCommService> Create(IArdNetServer ArdServer, int PlayerCount)
        {
            var svc = new TankSimCommService(ArdServer, PlayerCount);
            var udpIP = await ArdServer.GetUdpAddrAsync();
            svc.GameID = udpIP.Port.ToString();
            return svc;
        }

        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="ArdServer"></param>
        /// <param name="PlayerCount"></param>
        private TankSimCommService(IArdNetServer ArdServer, int PlayerCount)
        {
            if (ArdServer is null)
            {
                throw new ArgumentNullException(nameof(ArdServer));
            }

            this.ArdServer = ArdServer;
            CmdFacade = new OperatorCmdFacade(ArdServer);
            this.PlayerCountTarget = PlayerCount;
            _roleSets = OperatorRoleSets.GetDistributionSets(PlayerCount).ToList();
            _roleSets.Randomize();
            _playerWaiter = new CountdownEvent(PlayerCount);

            ArdServer.TcpEndpointConnected += ArdServer_TcpEndpointConnected;
            ArdServer.TcpEndpointDisconnected += ArdServer_TcpEndpointDisconnected;
            ArdServer.TcpQueryTable.Register(
                Constants.Queries.ControllerInit.GetOperatorRoles,
                ArdQry_RoleRequest);
            ArdServer.TcpCommandTable.Register(
                Constants.Commands.ControllerInit.SetClientName,
                ArdCmd_SetName);
        }

        private void ArdServer_TcpEndpointConnected(object Sender, IConnectedSystemEndpoint e)
        {
            //Pattern: State
            Debug.WriteLine($"Connected: {e.Endpoint}");
            var state = new TankControllerState();
            e.UserState = state;
            lock (e.SyncRoot)
            {
                state.Name = $"Anon{e.Endpoint.Port}";
            }
            _ = _connectedSystems.TryAdd(e.Endpoint, e);
            _ = Interlocked.Increment(ref _playerCountCurrent);
        }

        private void ArdServer_TcpEndpointDisconnected(object Sender, ISystemEndpoint e)
        {
            Debug.WriteLine($"Disconnected: {e.Endpoint}");
            var state = (TankControllerState)e.UserState;

            lock (e.SyncRoot)
            {
                if (state.IsReady)
                {
                    _playerWaiter.AddCount();
                    lock (_roleLock)
                    {
                        _roleSets.Add(state.Roles);
                        _roleSets.Randomize();
                    }
                }
            }
            _ = _connectedSystems.TryRemove(e.Endpoint, out _);
            _ = Interlocked.Decrement(ref _playerCountCurrent);
        }

        private void ArdQry_RoleRequest(object Sender, RequestResponderStateObject e)
        {
            var system = e.ConnectedSystem;
            var state = (TankControllerState)system.UserState;
            var uid = e.RequestArgs.Length > 0 ? e.RequestArgs[0] : "";
            lock (_roleLock)
            {
                if (!_opRoleCache.TryGetValue(uid, out var roles))
                {
                    roles = _roleSets.Pop();
                    if (uid.Length > 0)
                    {
                        _opRoleCache.Add(uid, roles);
                    }
                }
                e.Respond(roles.ToString());
                state.Roles = roles;
            }
        }

        private void ArdCmd_SetName(object Sender, RequestResponderStateObject e)
        {
            var system = e.ConnectedSystem;
            var state = (TankControllerState)system.UserState;
            lock (system.SyncRoot)
            {
                if ((e.RequestArgs?.Length ?? 0) > 0)
                {
                    state.Name = e.RequestArgs[0];
                }
                e.Respond(AsciiSymbols.ACK);
            }
            TankControllerReady?.Invoke(this, state);
            Debug.WriteLine($"Hi {state.Name} ({e.Endpoint})");
            lock (system.SyncRoot)
            {
                _ = _playerWaiter.Signal();
                state.IsReady = true;
            }
        }

        /// <summary>
        /// Wait for all players to connect
        /// </summary>
        public void WaitForPlayers()
        {
            _playerWaiter.Wait();
        }

        /// <summary>
        /// Wait for all players to connect
        /// </summary>
        /// <param name="Token"></param>
        public void WaitForPlayers(CancellationToken Token)
        {
            _playerWaiter.Wait(Token);
        }

        /// <summary>
        /// Wait for all players to connect
        /// </summary>
        /// <param name="Timeout"></param>
        public void WaitForPlayers(TimeSpan Timeout)
        {
            _ = _playerWaiter.Wait(Timeout);
        }

        /// <summary>
        /// Get task that triggers when all clients are connected.
        /// </summary>
        /// <returns></returns>
        public Task GetConnectionTask()
        {
            //dirty hack
            return Task.Run(() =>
            {
                _playerWaiter.Wait();
            });
        }

        /// <summary>
        /// Get task that triggers when all clients are connected.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public Task GetConnectionTask(CancellationToken Token)
        {
            //dirty hack
            return Task.Run(() =>
            {
                _playerWaiter.Wait(Token);
            }, Token);
        }

        /// <summary>
        /// Get task that triggers when all clients are connected.
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public Task GetConnectionTask(TimeSpan Timeout)
        {
            //dirty hack
            return Task.Run(() =>
            {
                _ = _playerWaiter.Wait(Timeout);
            });
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _playerWaiter.Dispose();
            CmdFacade.Dispose();
            ArdServer.TcpEndpointConnected -= ArdServer_TcpEndpointConnected;
            ArdServer.TcpEndpointDisconnected -= ArdServer_TcpEndpointDisconnected;
            _ = ArdServer.TcpQueryTable.Unregister(
                Constants.Queries.ControllerInit.GetOperatorRoles,
                ArdQry_RoleRequest);
            _ = ArdServer.TcpCommandTable.Unregister(
                Constants.Commands.ControllerInit.SetClientName,
                ArdCmd_SetName);
            GC.SuppressFinalize(this);
        }
    }
}
