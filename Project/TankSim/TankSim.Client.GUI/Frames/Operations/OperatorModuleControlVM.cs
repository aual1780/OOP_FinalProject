using ArdNet;
using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TankSim.Client.OperatorModules;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class OperatorModuleControlVM : ViewModelBase, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private readonly IOperatorModuleFactory _moduleFactory;
        private IConnectedSystemEndpoint _gameHost;
        private OperatorRoles _roles = 0;
        private IOperatorModuleCollection _moduleCollection;
        private readonly CancellationTokenSource _initSyncTokenSrc = new CancellationTokenSource();

        public OperatorRoles Roles
        {
            get => _roles;
            set => SetField(ref _roles, value);
        }
        public IConnectedSystemEndpoint GameHost
        {
            get => _gameHost;
            set
            {
                if (SetField(ref _gameHost, value))
                {
                    InvokePropertyChanged(nameof(GameHostEndpoint));
                }
            }
        }
        public string GameHostEndpoint
        {
            get => _gameHost?.Endpoint?.ToString() ?? "N/A";
        }
        public IOperatorModuleCollection ModuleCollection
        {
            get => _moduleCollection;
            set
            {
                if (SetField(ref _moduleCollection, value))
                {
                    InvokePropertyChanged(nameof(ModuleCtrlCollection));
                }
            }
        }
        public IEnumerable<UserControl> ModuleCtrlCollection
        {
            get => _moduleCollection?.OfType<UserControl>();
        }

        public OperatorModuleControlVM(IArdNetClient ArdClient, IOperatorModuleFactory ModuleFactory)
        {
            _ardClient = ArdClient;
            _moduleFactory = ModuleFactory;
            _ardClient.TcpEndpointConnected += ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected += ArdClient_TcpEndpointDisconnected;
        }

        private void ArdClient_TcpEndpointConnected(object Sender, ConnectedSystemEndpointArgs e)
        {
            GameHost = e.System;
        }

        private void ArdClient_TcpEndpointDisconnected(object Sender, SystemEndpointArgs e)
        {
            GameHost = null;
        }

        public override async Task InitializeAsync()
        {
            _ = await _ardClient.ConnectAsync();
            var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
            var request = new AsyncRequestPushedArgs(qry, null, _initSyncTokenSrc.Token, Timeout.InfiniteTimeSpan);
            try
            {
                var task = _ardClient.SendTcpQueryAsync(request);
                var response = await task;
                var responseStr = response?.Single()?.Response ?? "0";
                Roles = Enum.Parse<OperatorRoles>(responseStr);
                ModuleCollection = _moduleFactory.GetModuleCollection(Roles);
            }
            catch (OperationCanceledException)
            {
                //noop
                //early shutdown
            }
        }

        public void Dispose()
        {
            try
            {
                _initSyncTokenSrc.Cancel();
                _initSyncTokenSrc.Dispose();
            }
            catch { }
            _ardClient.TcpEndpointConnected -= ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected -= ArdClient_TcpEndpointDisconnected;
            _moduleCollection?.Dispose();
        }
    }
}
