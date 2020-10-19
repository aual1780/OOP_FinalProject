using ArdNet;
using ArdNet.Client;
using ArdNet.Topics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
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
using TankSim.OperatorCmds;
using TankSim.OperatorDelegates;
using TIPC.Core.Tools.Threading;

namespace TankSim.Client.GUI.OperatorModules
{
    /// <summary>
    /// Interaction logic for GuiDriverCtrl.xaml
    /// </summary>
    [OperatorRole(OperatorRoles.Driver | OperatorRoles.Navigator)]
    public partial class GuiDriver : UserControl, IOperatorModule, INotifyPropertyChanged
    {
        private readonly IArdNetClient _ardClient;
        private readonly TankMovementDelegate _movDelegate;
        private MovementDirection _dir;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool DirNW => _dir == (MovementDirection.North | MovementDirection.West);
        public bool DirN => _dir == (MovementDirection.North);
        public bool DirNE => _dir == (MovementDirection.North | MovementDirection.East);

        public bool DirW => _dir == (MovementDirection.West);
        public bool DirE => _dir == (MovementDirection.East);

        public bool DirSW => _dir == (MovementDirection.South | MovementDirection.West);
        public bool DirS => _dir == (MovementDirection.South);
        public bool DirSE => _dir == (MovementDirection.South | MovementDirection.East);


        public GuiDriver(IArdNetClient ArdClient)
        {
            DataContext = this;
            _ardClient = ArdClient;
            _movDelegate = new TankMovementDelegate(_ardClient);
            this.Loaded += GuiDriverCtrl_Loaded;
            InitializeComponent();
        }

        private void GuiDriverCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            _movDelegate.MovementChanged += MovDelegate_MovementChanged;
        }

        private void MovDelegate_MovementChanged(IConnectedSystemEndpoint Endpoint, MovementDirection Dir)
        {
            _dir = Dir;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirNW)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirN)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirNE)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirW)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirE)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirSW)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirS)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirSE)));
        }



        public void HandleInput(IOperatorInputMsg Input)
        {
            //noop
        }

        public void Dispose()
        {
            _movDelegate?.Dispose();
        }
    }
}
