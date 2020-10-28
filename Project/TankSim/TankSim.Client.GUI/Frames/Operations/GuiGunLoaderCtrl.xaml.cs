using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiGunLoaderCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.GunLoader)]
    public partial class GuiGunLoaderCtrl : UserControl, IOperatorUIModule
    {
        public GuiGunLoaderCtrl()
        {
            InitializeComponent();
        }
    }
}
