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

namespace TankSim.Client.GUI.OperatorModules
{
    /// <summary>
    /// Interaction logic for GuiDriverCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.Driver)]
    public partial class GuiDriverCtrl : UserControl, IOperatorModule
    {
        GuiDriver _moduleWorker;

        public GuiDriverCtrl(GuiDriver _moduleWorker)
        {
            this._moduleWorker = _moduleWorker;
            InitializeComponent();
        }

        public void HandleInput(IOperatorInputMsg Input)
        {
            _moduleWorker.HandleInput(Input);
        }

        public void Dispose()
        {
            _moduleWorker.Dispose();
        }
    }
}
