using System.Windows.Controls;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.GUI.Frames.Operations
{
    /// <summary>
    /// Interaction logic for GuiRangeFinderCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.RangeFinder)]
    public partial class GuiRangeFinderCtrl : UserControl, IOperatorUIModule
    {
        public GuiRangeFinderCtrl()
        {
            InitializeComponent();
        }
    }
}
