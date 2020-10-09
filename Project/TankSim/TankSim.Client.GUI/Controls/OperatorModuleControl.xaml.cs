using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.Extensions;
using TankSim.Client.GUI.OperatorModules;
using TankSim.Client.GUI.ViewModels;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Controls
{
    /// <summary>
    /// Interaction logic for OperatorModuleControl.xaml
    /// </summary>
    public partial class OperatorModuleControl : UserControl, IDisposable
    {
        private readonly OperatorModuleControlVM _vm;

        public OperatorModuleControl(OperatorModuleControlVM vm)
        {
            this.Initialized += OperatorModuleControl_Initialized;
            this.Loaded += OperatorModuleControl_Loaded;
            this.Unloaded += OperatorModuleControl_Unloaded;
            this._vm = vm;
            this.DataContext = _vm;
            InitializeComponent();
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
            _vm.ModuleCollection?.SendInput(input);
        }

        private void OperatorModuleControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }
            var consoleKey = e.Key.ToConsoleKey();
            var input = new OperatorInputMsg(consoleKey, KeyInputType.KeyDown);
            _vm.ModuleCollection?.SendInput(input);
        }

        private async void OperatorModuleControl_Initialized(object sender, EventArgs e)
        {
            await Task.Run(_vm.InitializeAsync);
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
