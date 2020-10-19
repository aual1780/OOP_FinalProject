using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiGunLoaderCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.FireControl)]
    public partial class GuiGunLoaderCtrl : UserControl, IOperatorModule
    {
        public GuiGunLoaderCtrl()
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
