using ArdNet;
using ArdNet.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TankSim.GameHost.Extensions;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost
{
    /// <summary>
    /// 
    /// </summary>
    public class TankSimCommService : IDisposable
    {
        ConcurrentDictionary<IPEndPoint, IConnectedSystemEndpoint> _connectedSystems = new ConcurrentDictionary<IPEndPoint, IConnectedSystemEndpoint>();
        private readonly object _roleLock = new object();
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
        public string GameID { get; }
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
        /// Create instance
        /// </summary>
        /// <param name="ArdServer"></param>
        /// <param name="PlayerCount"></param>
        public TankSimCommService(IArdNetServer ArdServer, int PlayerCount)
        {
            if (ArdServer is null)
            {
                throw new ArgumentNullException(nameof(ArdServer));
            }

            this.ArdServer = ArdServer;
            GameID = ArdServer.NetConfig.UDP.AppID.Split('.').Last();
            CmdFacade = new OperatorCmdFacade(ArdServer);
            this.PlayerCountTarget = PlayerCount;
            _roleSets = OperatorRoleSets.GetDistributionSets(PlayerCount).ToList();
            _roleSets.Randomize();
            _playerWaiter = new CountdownEvent(PlayerCount);

            ArdServer.TcpEndpointConnected += ArdServer_TcpEndpointConnected;
            ArdServer.TcpEndpointDisconnected += ArdServer_TcpEndpointDisconnected;
            ArdServer.TcpQueryReceived += ArdServer_TcpQueryReceived;
            ArdServer.TcpCommandReceived += ArdServer_TcpCommandReceived;
        }

        private void ArdServer_TcpEndpointConnected(object Sender, IConnectedSystemEndpoint e)
        {
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

        private void ArdServer_TcpQueryReceived(object Sender, ArdNet.Messaging.RequestReceivedArgs e)
        {
            if (e.Request == Constants.Queries.ControllerInit.GetOperatorRoles)
            {
                var system = e.ConnectedSystem;
                var state = (TankControllerState)system.UserState;
                lock (_roleLock)
                {
                    var roleSet = _roleSets.Pop();
                    ArdServer.SendTcpQueryResponse(e, roleSet.ToString());
                    state.Roles = roleSet;
                }
                lock (system.SyncRoot)
                {
                    _ = _playerWaiter.Signal();
                    state.IsReady = true;
                }
            }
        }

        private void ArdServer_TcpCommandReceived(object Sender, ArdNet.Messaging.RequestReceivedArgs e)
        {
            if (e.Request == Constants.Commands.ControllerInit.SetClientName)
            {
                var system = e.ConnectedSystem;
                var state = (TankControllerState)system.UserState;
                lock (system.SyncRoot)
                {
                    if ((e.RequestArgs?.Length ?? 0) > 0)
                    {
                        state.Name = e.RequestArgs[0];
                    }
                    ArdServer.SendTcpCommandResponse(e, CtrlSymbols.ACK);
                }
                Debug.WriteLine($"Hi {state.Name} ({e.Endpoint})");
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
            _playerWaiter.Wait(Timeout);
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
            });
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
                _playerWaiter.Wait(Timeout);
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
            ArdServer.TcpQueryReceived -= ArdServer_TcpQueryReceived;
            ArdServer.TcpCommandReceived -= ArdServer_TcpCommandReceived;
        }
    }
}
