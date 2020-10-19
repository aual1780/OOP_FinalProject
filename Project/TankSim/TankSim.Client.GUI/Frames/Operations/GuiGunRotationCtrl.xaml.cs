using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiGunRotationCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.GunRotation)]
    public partial class GuiGunRotationCtrl : UserControl, IOperatorModule
    {
        public GuiGunRotationCtrl()
        {
            InitializeComponent();
        }

        public void HandleInput(IOperatorInputMsg Input)
        {
            //noop
        }

        public void Dispose()
        {
            //noop
        }
    }
}
