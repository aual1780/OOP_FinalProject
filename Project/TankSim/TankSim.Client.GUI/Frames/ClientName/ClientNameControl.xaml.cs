using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TankSim.Client.GUI.Frames.ClientName
{
    /// <summary>
    /// Interaction logic for ClientNameControl.xaml
    /// </summary>
    public partial class ClientNameControl : UserControl
    {
        private readonly ClientNameControlVM _vm;

        public ClientNameControl(ClientNameControlVM vm)
        {
            _vm = vm;
            this.DataContext = _vm;
            this.Initialized += ClientNameControl_Initialized;
            this.Loaded += ClientNameControl_Loaded;
            InitializeComponent();
        }

        private async void ClientNameControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        private void ClientNameControl_Loaded(object sender, RoutedEventArgs e)
        {
            _ = txt_Username.Focus();
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

#pragma warning disable IDE1006 // Naming Styles
        private void txt_Username_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Button_Click(null, null);
            }
        }
    }
}
