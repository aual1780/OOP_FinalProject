using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.Tools;
using TankSim.Config;

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
        private readonly object _vmInitLock = new object();
        private Task _vmInitTask = null;


        public OperatorModuleControl(OperatorModuleControlVM vm, IOptions<KeyBindingConfig> keyConfig)
        {
            this._vm = vm;
            this.DataContext = _vm;
            this.Initialized += OperatorModuleControl_Initialized;
            this.Loaded += OperatorModuleControl_Loaded;
            this.Unloaded += OperatorModuleControl_Unloaded;
            if (keyConfig.Value.EnableGlobalHooks)
            {
                _globalHook = new GlobalKeyHook(true);
            }
            InitializeComponent();
        }

        public void BeginVmInit()
        {
            if (_vmInitTask is not null)
            {
                return;
            }
            lock (_vmInitLock)
            {
                if (_vmInitTask is not null)
                {
                    return;
                }
                _vmInitTask = _vm.InitializeAsync();
            }
        }

        async void OperatorModuleControl_Initialized(object sender, EventArgs e)
        {
            BeginVmInit();
            await _vmInitTask;
        }

        void OperatorModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            _myWindow = Window.GetWindow(this);
            _myWindow.KeyDown += OperatorModuleControl_KeyDown;
            _myWindow.KeyUp += OperatorModuleControl_KeyUp;
            if (_globalHook != null)
            {
                _globalHook.KeyDown += GlobalHook_KeyDown;
                _globalHook.KeyUp += GlobalHook_KeyUp;
            }
        }

        void OperatorModuleControl_Unloaded(object sender, RoutedEventArgs e) => this.Dispose();

        void OperatorModuleControl_KeyDown(object sender, KeyEventArgs e) => _vm.HandleKeyEvent(e, KeyInputType.KeyDown);
        void OperatorModuleControl_KeyUp(object sender, KeyEventArgs e) => _vm.HandleKeyEvent(e, KeyInputType.KeyUp);
        void GlobalHook_KeyDown(RawKeyEventArgs e)
        {
            if (_myWindow.IsActive)
            {
                return;
            }
            _vm.HandleKeyEvent(e, KeyInputType.KeyDown);
        }
        void GlobalHook_KeyUp(RawKeyEventArgs e)
        {
            if (_myWindow.IsActive)
            {
                return;
            }
            _vm.HandleKeyEvent(e, KeyInputType.KeyUp);
        }

        private void GamepadRadio_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            var selectedController = mi.Header.ToString();
            foreach (var itm in mnu_GamepadRadio.Items)
            {
                if (itm != sender)
                {
                    ((MenuItem)itm).IsChecked = false;
                }
            }
            if (int.TryParse(selectedController, out var idx))
            {
                _vm.GamepadIndex = idx;
            }
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
