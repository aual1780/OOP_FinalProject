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
using TIPC.Core.Tools.Threading;
using TIPC.Core.Collections.Generic;
using TankSim.Client.Services;

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
        private int _gamepadIndex = 1;
        private readonly IGamepadService _gamepadService;
        private readonly IRoleResolverService _roleService;
        private readonly CancelThread<object> _inputProcessorThread;
        readonly PriorityBlockingQueue<(Key, KeyInputType)> _keyQueue = new PriorityBlockingQueue<(Key, KeyInputType)>();


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
        public int GamepadIndex
        {
            get => _gamepadIndex;
            set
            {
                if (!_gamepadService.TrySetControllerIndex(value - 1))
                {
                    return;
                }
                _ = SetField(ref _gamepadIndex, value);
            }
        }


        public OperatorModuleControlVM(
            IArdNetClient ArdClient,
            IOperatorModuleFactory<IOperatorInputModule> InputModuleFactory,
            IOperatorModuleFactory<IOperatorUIModule> UIModuleFactory,
            IGamepadService GamepadService,
            IRoleResolverService RoleService)
        {
            _ardClient = ArdClient;
            _inputModuleFactory = InputModuleFactory;
            _uiModuleFactory = UIModuleFactory;
            _inputProcessorThread = new CancelThread<object>(KeyProcessingLoop);
            _inputProcessorThread.Start();
            _ardClient.TcpEndpointConnected += ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected += ArdClient_TcpEndpointDisconnected;
            _gamepadService = GamepadService;
            _roleService = RoleService;
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
            try
            {
                Roles = await _roleService.GetRolesAsync();
                InputModuleCollection = _inputModuleFactory.GetModuleCollection(Roles);
                UIModuleCollection = _uiModuleFactory.GetModuleCollection(Roles).OfType<UserControl>();
                _gamepadService.SetRoles(Roles);
            }
            catch (OperationCanceledException)
            {
                //noop
                //early shutdown
            }
        }

        public void HandleKeyEvent(KeyEventArgs e, KeyInputType PressType)
        {
            if (e.IsRepeat)
            {
                return;
            }
            _ = _keyQueue.TryAdd((e.Key, PressType));
        }

        public void HandleKeyEvent(RawKeyEventArgs e, KeyInputType PressType)
        {
            if (e.IsRepeat)
            {
                return;
            }
            _ = _keyQueue.TryAdd((e.Key, PressType));
        }

        private void KeyProcessingLoop(object state, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!_keyQueue.TryTake(out var tuple, token))
                {
                    return;
                }
                var key = tuple.Item1;
                var pressType = tuple.Item2;

                var consoleKey = key.ToConsoleKey();
                OperatorInputMsg input;
                //keydown
                if (pressType == KeyInputType.KeyDown)
                {
                    input = new OperatorInputMsg(consoleKey, KeyInputType.KeyDown);
                }
                //keyup
                else
                {
                    input = new OperatorInputMsg(consoleKey, KeyInputType.KeyUp);
                }
                InputModuleCollection?.SendInput(input);
            }
        }

        public void Dispose()
        {
            _ardClient.TcpEndpointConnected -= ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected -= ArdClient_TcpEndpointDisconnected;
            _inputProcessorThread.Dispose();
            _inputModuleCollection?.DisposeAll();
        }
    }
}
