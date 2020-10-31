using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.Tools;
using TankSim.Config;
using TIPC.Core.Tools.Extensions;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for OperatorModuleControl.xaml
    /// </summary>
    public partial class OperatorModuleControl : UserControl, IDisposable
    {
        private readonly OperatorModuleControlVM _vm;
        private readonly GlobalKeyHook _globalHook;
        private Window _myWindow;

        public OperatorModuleControl(OperatorModuleControlVM vm, IOptions<KeyBindingConfig> keyConfig)
        {
            this._vm = vm;
            this.DataContext = _vm;
            this.Initialized += OperatorModuleControl_Initialized;
            this.Loaded += OperatorModuleControl_Loaded;
            this.Unloaded += OperatorModuleControl_Unloaded;
            if (keyConfig.Value.EnableGlobalHooks)
            {
                _globalHook = new GlobalKeyHook();
                _globalHook.KeyDown += GlobalHook_KeyDown;
                _globalHook.KeyUp += GlobalHook_KeyUp;
            }
            InitializeComponent();
        }

        async void OperatorModuleControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        void OperatorModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            _myWindow = Window.GetWindow(this);
            _myWindow.KeyDown += OperatorModuleControl_KeyDown;
            _myWindow.KeyUp += OperatorModuleControl_KeyUp;
            _globalHook?.Hook();
        }

        void OperatorModuleControl_Unloaded(object sender, RoutedEventArgs e) => this.Dispose();

        void OperatorModuleControl_KeyDown(object sender, KeyEventArgs e) => _vm.HandleKeyEvent(e, KeyInputType.KeyDown);
        void OperatorModuleControl_KeyUp(object sender, KeyEventArgs e) => _vm.HandleKeyEvent(e, KeyInputType.KeyUp);
        void GlobalHook_KeyDown(object sender, RawKeyEventArgs e)
        {
            if (_myWindow.IsActive)
            {
                return;
            }
            _vm.HandleKeyEvent(e, KeyInputType.KeyDown);
        }
        void GlobalHook_KeyUp(object sender, RawKeyEventArgs e)
        {
            if (_myWindow.IsActive)
            {
                return;
            }
            _vm.HandleKeyEvent(e, KeyInputType.KeyUp);
        }

        public void Dispose()
        {
            try
            {
                var window = Window.GetWindow(this);
                window.KeyUp -= OperatorModuleControl_KeyUp;
                window.KeyDown -= OperatorModuleControl_KeyDown;
                _globalHook?.Dispose();
            }
            catch
            {
                //noop
            }
            _vm.Dispose();
        }
    }
}
