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
        Window _myWindow;

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
            _myWindow = Window.GetWindow(this);
            _ = txt_Username.Focus();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.IsUIEnabled = false;
            _myWindow.Cursor = Cursors.Wait;
            try
            {
                await _vm.SubmitName();
            }
            catch
            {
                _vm.StatusMsg = "Unable to submit name";
            }
            finally
            {
                _vm.IsUIEnabled = false;
                _myWindow.Cursor = Cursors.Arrow;
            }
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
