using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TankSim.Client.Uno;
using TankSim.Client.Uno.Shared.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TankSim.Client.GUI.Frames.GameScope
{
    /// <summary>
    /// Interaction logic for GameScopeControl.xaml
    /// </summary>
    public partial class GameScopeControl : UserControl
    {
        readonly GameScopeControlVM _vm;
        Window _myWindow;

        public GameScopeControl()
        {
            _vm = DiContainer.Instance().GetRequiredService<GameScopeControlVM>();
            this.DataContext = _vm;
            this.Loading += GameScopeControl_Loading;
            this.Loaded += GameScopeControl_Loaded;
        }



        private async void GameScopeControl_Loading(DependencyObject sender, object args)
        {
            await _vm.InitializeAsync();
        }

        private void GameScopeControl_Loaded(object sender, RoutedEventArgs e)
        {
            _myWindow = Window.Current;
            _ = txt_GameID.Focus(FocusState.Keyboard);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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

                _myWindow.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 0);
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
                _myWindow.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
                txt_GameID.Text = "";
                _ = txt_GameID.Focus(FocusState.Keyboard);
            }
        }

        public Task<IServiceScope> GetGameScopeAsync()
        {
            return _vm.IdTaskSource.Task;
        }

#pragma warning disable IDE1006 // Naming Styles
        private void txt_GameID_KeyDown(object sender, KeyRoutedEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                Button_Click(null, null);
            }
        }
    }
}
