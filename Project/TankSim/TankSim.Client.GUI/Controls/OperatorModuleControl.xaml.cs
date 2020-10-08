using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using TankSim.Client.GUI.ViewModels;

namespace TankSim.Client.GUI.Controls
{
    /// <summary>
    /// Interaction logic for OperatorModuleControl.xaml
    /// </summary>
    public partial class OperatorModuleControl : UserControl
    {
        private readonly OperatorModuleControlVM _vm;

        public OperatorModuleControl(OperatorModuleControlVM vm)
        {
            this.Initialized += OperatorModuleControl_Initialized;
            this._vm = vm;
            this.DataContext = _vm;
            InitializeComponent();
        }

        private async void OperatorModuleControl_Initialized(object sender, EventArgs e)
        {
            await Task.Run(_vm.InitializeAsync);
        }

        public Task ControllerExecAsync()
        {
            return _vm.OpControllerTaskSource.Task;
        }
    }
}
