using System;
using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiGunLoaderCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.GunLoader)]
    public partial class GuiGunLoaderCtrl : UserControl, IOperatorUIModule, IDisposable
    {
        private readonly IDisposable _vm;

        public GuiGunLoaderCtrl(GuiGunLoaderVM vm)
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
