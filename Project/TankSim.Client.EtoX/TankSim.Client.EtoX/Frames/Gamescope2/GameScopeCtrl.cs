using System;
using Eto.Forms;
using Eto.Drawing;
using System.Threading.Tasks;
using TankSim.Client.EtoX.Frames.GameScope;

namespace TankSim.Client.EtoX.Frames.GameScope2
{
    public partial class GameScopeCtrl : Panel
    {
        readonly GameScopeVM _vm;
        Task _initTask;

        public GameScopeCtrl(GameScopeVM VM)
        {
            _vm = VM;
            this.DataContext = _vm;
            lbl_GameTitle.Text = TankSim.Constants.GameName;
            _ = txt_GameID.TextBinding.BindDataContext((GameScopeVM x) => x.GameID);
            _ = txt_GameID.BindDataContext(x => x.Enabled, (GameScopeVM x) => x.IsUIEnabled);
            lbl_ConnectionStatus.BindDataContext(x => x.Text, (GameScopeVM x) => x.StatusMsg);
            _ = btn_Submit.BindDataContext(x => x.Enabled, (GameScopeVM x) => x.IsUIEnabled);
            InitializeComponent();
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            _initTask = _vm.InitializeAsync();
        }

        protected async override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            await _initTask;
            txt_GameID.Focus();
        }

        private async void Btn_Submit_Click(object sender, EventArgs e)
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

                ParentWindow.Cursor = Cursors2.Wait;
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
                ParentWindow.Cursor = Cursors.Arrow;
                txt_GameID.Text = "";
                txt_GameID.Focus();
            }
        }

        private void Txt_GameID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                btn_Submit.PerformClick();
            }
        }
    }
}
