using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiFireControlCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.FireControl)]
    public partial class GuiFireControlCtrl : UserControl, IOperatorModule
    {
        public GuiFireControlCtrl()
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
