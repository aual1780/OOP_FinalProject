using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TankSim.Client.GUI.Frames.GameScope
{
    /// <summary>
    /// Interaction logic for GameScopeControl.xaml
    /// </summary>
    public partial class GameScopeControl : UserControl
    {
        readonly GameScopeControlVM _vm;
        Window _myWindow;

        public GameScopeControl(GameScopeControlVM vm)
        {
            _vm = vm;
            this.DataContext = _vm;
            this.Initialized += GameScopeControl_Initialized;
            this.Loaded += GameScopeControl_Loaded;
            InitializeComponent();
        }

        private async void GameScopeControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        private void GameScopeControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _myWindow = Window.GetWindow(this);
            _ = txt_GameID.Focus();
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = (Button)sender;
            try
            {
                _vm.IsUIEnabled = false;
                if (!GameIdGenerator.Validate(_vm.GameID))
                {
                    _vm.StatusMsg = "Invalid Game ID";
                    return;
                }

                _myWindow.Cursor = Cursors.Wait;
                var scope = await Task.Run(_vm.ValidateGameID);
                if (scope != null)
                {
                    _ = _vm.IdTaskSource.TrySetResult(scope);
                }
            }
            catch
            {
                _vm.StatusMsg = "Failed to Connect";
                return;
            }
            finally
            {
                _vm.IsUIEnabled = true;
                _myWindow.Cursor = Cursors.Arrow;
                txt_GameID.Text = "";
                _ = txt_GameID.Focus();
            }
        }

        public Task<IServiceScope> GetGameScopeAsync()
        {
            return _vm.IdTaskSource.Task;
        }

#pragma warning disable IDE1006 // Naming Styles
        private void txt_GameID_KeyDown(object sender, KeyEventArgs e)
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
