using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.ViewModels;

namespace TankSim.Client.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ClientNameControl.xaml
    /// </summary>
    public partial class ClientNameControl : UserControl
    {
        private readonly ClientNameControlVM _vm;

        public ClientNameControl(ClientNameControlVM vm)
        {
            this.Initialized += ClientNameControl_Initialized;
            _vm = vm;
            this.DataContext = _vm;
            InitializeComponent();
        }

        private async void ClientNameControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.IsUIEnabled = false;
            Cursor = Cursors.Wait;
            _vm.ArdClient.SendTcpCommand(Constants.Commands.ControllerInit.SetClientName, _vm.Username);
            _ = _vm.NameTaskSource.TrySetResult(null);
        }

        public Task SendClientNameAsync()
        {
            return _vm.NameTaskSource.Task;
        }
    }
}
