using ArdNet;
using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.Extensions;
using TankSim.Client.OperatorModules;
using TIPC.Core.ComponentModel;
using TIPC.Core.Tools.Extensions.IEnumerable;
using TankSim.Client.Extensions;
using TankSim.Client.GUI.Tools;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class OperatorModuleControlVM : ViewModelBase, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private readonly IOperatorModuleFactory<IOperatorInputModule> _inputModuleFactory;
        private readonly IOperatorModuleFactory<IOperatorUIModule> _uiModuleFactory;
        private IEnumerable<IOperatorInputModule> _inputModuleCollection;
        private IEnumerable<UserControl> _uiModuleCollection;
        private IConnectedSystemEndpoint _gameHost;
        private OperatorRoles _roles = 0;
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
        public IEnumerable<IOperatorInputModule> InputModuleCollection
        {
            get => _inputModuleCollection;
            set => SetField(ref _inputModuleCollection, value);
        }
        public IEnumerable<UserControl> UIModuleCollection
        {
            get => _uiModuleCollection;
            set => SetField(ref _uiModuleCollection, value);
        }

        public OperatorModuleControlVM(
            IArdNetClient ArdClient,
            IOperatorModuleFactory<IOperatorInputModule> InputModuleFactory,
            IOperatorModuleFactory<IOperatorUIModule> UIModuleFactory)
        {
            _ardClient = ArdClient;
            _inputModuleFactory = InputModuleFactory;
            _uiModuleFactory = UIModuleFactory;
            _ardClient.TcpEndpointConnected += ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected += ArdClient_TcpEndpointDisconnected;
        }

        private void ArdClient_TcpEndpointConnected(object Sender, IConnectedSystemEndpoint e)
        {
            GameHost = e;
        }

        private void ArdClient_TcpEndpointDisconnected(object Sender, ISystemEndpoint e)
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
                InputModuleCollection = _inputModuleFactory.GetModuleCollection(Roles);
                UIModuleCollection = _uiModuleFactory.GetModuleCollection(Roles).OfType<UserControl>();
            }
            catch (OperationCanceledException)
            {
                //noop
                //early shutdown
            }
        }

        public void HandleKeyEvent(KeyEventArgs e, KeyInputType PressType)
        {
            var consoleKey = e.Key.ToConsoleKey();
            OperatorInputMsg input;
            //keydown
            if (PressType == KeyInputType.KeyDown)
            {
                if (e.IsRepeat)
                {
                    return;
                }
                input = new OperatorInputMsg(consoleKey, KeyInputType.KeyDown);
            }
            //keyup
            else
            {
                input = new OperatorInputMsg(consoleKey, KeyInputType.KeyUp);
            }
            InputModuleCollection?.SendInput(input);
        }

        public void HandleKeyEvent(RawKeyEventArgs e, KeyInputType PressType)
        {
            var consoleKey = e.Key.ToConsoleKey();
            OperatorInputMsg input;
            //keydown
            if (PressType == KeyInputType.KeyDown)
            {
                if (e.IsRepeat)
                {
                    return;
                }
                input = new OperatorInputMsg(consoleKey, KeyInputType.KeyDown);
            }
            //keyup
            else
            {
                input = new OperatorInputMsg(consoleKey, KeyInputType.KeyUp);
            }
            InputModuleCollection?.SendInput(input);
        }

        public void Dispose()
        {
            try
            {
                _initSyncTokenSrc.Cancel();
                Thread.MemoryBarrier();
                _initSyncTokenSrc.Dispose();
            }
            catch { }
            _ardClient.TcpEndpointConnected -= ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected -= ArdClient_TcpEndpointDisconnected;
            _inputModuleCollection?.DisposeAll();
        }
    }
}
