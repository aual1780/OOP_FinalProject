using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiFireControlCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.FireControl)]
    public partial class GuiFireControlCtrl : UserControl, IOperatorUIModule
    {
        public GuiFireControlCtrl()
        {
            InitializeComponent();
        }
    }
}
