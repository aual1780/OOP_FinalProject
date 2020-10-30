using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
