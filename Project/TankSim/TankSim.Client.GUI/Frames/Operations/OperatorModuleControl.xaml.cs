using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        async void OperatorModuleControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        void OperatorModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyUp += OperatorModuleControl_KeyUp;
            window.KeyDown += OperatorModuleControl_KeyDown;
        }

        void OperatorModuleControl_Unloaded(object sender, RoutedEventArgs e) => this.Dispose();

        void OperatorModuleControl_KeyUp(object sender, KeyEventArgs e) => _vm.HandleKeyEvent(e, KeyInputType.KeyUp);

        void OperatorModuleControl_KeyDown(object sender, KeyEventArgs e) => _vm.HandleKeyEvent(e, KeyInputType.KeyDown);

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
