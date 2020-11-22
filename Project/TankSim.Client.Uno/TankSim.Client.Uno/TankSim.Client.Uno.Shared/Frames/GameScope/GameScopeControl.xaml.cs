using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TankSim.Client.Uno.Frames.GameScope;
using TankSim.Client.Uno.Shared.Extensions;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TankSim.Client.Uno.Skia.Gtk.Frames.GameScope
{
    /// <summary>
    /// Interaction logic for GameScopeControl.xaml
    /// </summary>
    public partial class GameScopeControl : UserControl, IGameScopeControl
    {
        readonly GameScopeControlVM _vm;
        Task _initTask;
        Window _myWindow;
        TextBox txt_GameID;

        public GameScopeControl(GameScopeControlVM vm)
        {
            _vm = vm;
            this.DataContext = _vm;
            this.Loading += GameScopeControl_Loading;
            this.Loaded += GameScopeControl_Loaded;
        }

        private void GameScopeControl_Loading(object sender, RoutedEventArgs e)
        {
            _initTask = _vm.InitializeAsync();
        }

        private void GameScopeControl_Loaded(object sender, RoutedEventArgs e)
        {
            _myWindow = Window.Current;
            txt_GameID = this.FindControl<TextBox>(nameof(txt_GameID));
            _ = txt_GameID.Focus(FocusState.Keyboard);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await _initTask;
            var btn = (Button)sender;
            try
            {
                _vm.IsUIEnabled = false;
                if (!GameIdGenerator.Validate(_vm.GameID))
                {
                    _vm.StatusMsg = "Invalid Game ID";
                    return;
                }

                _myWindow.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Wait, 0);
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
                _myWindow.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                txt_GameID.Text = "";
                _ = txt_GameID.Focus(FocusState.Keyboard);
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
            if (e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                Button_Click(null, null);
            }
        }
    }
}
