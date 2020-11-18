using System;
using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiDriverCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.Driver | OperatorRoles.Navigator)]
    public partial class GuiDriver : UserControl, IOperatorUIModule, IDisposable
    {
        private readonly IDisposable _vm;

        public GuiDriver(GuiDriverVM vm)
        {
            _vm = vm;
            DataContext = _vm;
            InitializeComponent();
        }

        public void Dispose()
        {
            _vm.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
