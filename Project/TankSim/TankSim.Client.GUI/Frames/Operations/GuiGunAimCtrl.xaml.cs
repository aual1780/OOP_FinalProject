using System;
using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiGunAimCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.GunRotation | OperatorRoles.RangeFinder)]
    public partial class GuiGunAimCtrl : UserControl, IOperatorUIModule, IDisposable
    {
        private readonly IDisposable _vm;

        public GuiGunAimCtrl(GuiGunAimVM vm)
        {
            _vm = vm;
            DataContext = _vm;
            InitializeComponent();
        }

        public void Dispose()
        {
            _vm.Dispose();
        }
    }
}
