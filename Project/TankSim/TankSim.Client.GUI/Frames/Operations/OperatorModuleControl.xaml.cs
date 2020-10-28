using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.Extensions;
using TankSim.Client.OperatorModules;
using TankSim.Client.Extensions;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for OperatorModuleControl.xaml
    /// </summary>
    public partial class OperatorModuleControl : UserControl, IDisposable
    {
        private readonly OperatorModuleControlVM _vm;

        public OperatorModuleControl(OperatorModuleControlVM vm)
        {
            this._vm = vm;
            this.DataContext = _vm;
            this.Initialized += OperatorModuleControl_Initialized;
            this.Loaded += OperatorModuleControl_Loaded;
            this.Unloaded += OperatorModuleControl_Unloaded;
            InitializeComponent();
        }

        private async void OperatorModuleControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        private void OperatorModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyUp += OperatorModuleControl_KeyUp;
            window.KeyDown += OperatorModuleControl_KeyDown;
        }

        private void OperatorModuleControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        private void OperatorModuleControl_KeyUp(object sender, KeyEventArgs e)
        {
            var consoleKey = e.Key.ToConsoleKey();
            var input = new OperatorInputMsg(consoleKey, KeyInputType.KeyUp);
            _vm.InputModuleCollection?.SendInput(input);
        }

        private void OperatorModuleControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }
            var consoleKey = e.Key.ToConsoleKey();
            var input = new OperatorInputMsg(consoleKey, KeyInputType.KeyDown);
            _vm.InputModuleCollection?.SendInput(input);
        }

        public void Dispose()
        {
            try
            {
                var window = Window.GetWindow(this);
                window.KeyUp -= OperatorModuleControl_KeyUp;
                window.KeyDown -= OperatorModuleControl_KeyDown;
            }
            catch
            {
                //noop
            }
            _vm.Dispose();
        }
    }
}
