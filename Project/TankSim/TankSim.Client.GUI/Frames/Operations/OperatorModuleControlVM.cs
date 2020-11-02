using ArdNet;
using ArdNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.Extensions;
using TankSim.Client.OperatorModules;
using TIPC.Core.ComponentModel;
using TankSim.Client.GUI.Tools;
using TankSim.Client.Services;
using System.Windows;

namespace TankSim.Client.GUI.Frames.Operations
{
    public class OperatorModuleControlVM : ViewModelBase, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private readonly IRoleResolverService _roleService;
        private readonly IOperatorInputProcessorService _opInputService;
        private readonly IOperatorModuleFactory<IOperatorUIModule> _uiModuleFactory;
        private IEnumerable<UserControl> _uiModuleCollection;
        private IConnectedSystemEndpoint _gameHost;
        private OperatorRoles _roles = 0;
        private readonly IGamepadService _gamepadService;


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
        public IEnumerable<UserControl> UIModuleCollection
        {
            get => _uiModuleCollection;
            set => SetField(ref _uiModuleCollection, value);
        }
        public int GamepadIndex
        {
            get => _gamepadService.GamepadIndex + 1;
            set
            {
                if (_gamepadService.TrySetControllerIndex(value - 1))
                {
                    InvokePropertyChanged(nameof(GamepadIndex));
                }
            }
        }


        public OperatorModuleControlVM(
            IArdNetClient ArdClient,
            IRoleResolverService RoleService,
            IOperatorInputProcessorService OpInputService,
            IOperatorModuleFactory<IOperatorUIModule> UIModuleFactory,
            IGamepadService GamepadService)
        {
            _ardClient = ArdClient;
            _roleService = RoleService;
            _opInputService = OpInputService;
            _uiModuleFactory = UIModuleFactory;
            _ardClient.TcpEndpointConnected += ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected += ArdClient_TcpEndpointDisconnected;
            _gamepadService = GamepadService;
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
                var inptInitTask = _opInputService.Initialize();
                Roles = await _roleService.GetRolesAsync();
                await inptInitTask;
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
            var consoleKey = e.Key.ToConsoleKey();
            var arg = new OperatorInputEventArg(consoleKey, PressType);
            var msg = new OperatorInputMsg(this, arg);
            _ardClient.MessageHub.EnqueueMessage(msg);
        }

        public void HandleKeyEvent(RawKeyEventArgs e, KeyInputType PressType)
        {
            if (e.IsRepeat)
            {
                return;
            }
            var consoleKey = e.Key.ToConsoleKey();
            var arg = new OperatorInputEventArg(consoleKey, PressType);
            var msg = new OperatorInputMsg(this, arg);
            _ardClient.MessageHub.EnqueueMessage(msg);
        }

        public void Dispose()
        {
            _ardClient.TcpEndpointConnected -= ArdClient_TcpEndpointConnected;
            _ardClient.TcpEndpointDisconnected -= ArdClient_TcpEndpointDisconnected;
        }
    }
}
