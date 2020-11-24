using System;
using Eto.Forms;
using Eto.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TankSim.Client.EtoX.Frames.GameScope;

namespace TankSim.Client.EtoX
{
    public partial class MainForm : Form, IDisposable
    {
        readonly IServiceProvider _sp;
        readonly MainFormVM _vm;
        Task _vmInitTask;
        IServiceScope _scope;

        public MainForm(IServiceProvider ServiceProvider, MainFormVM VM)
        {
            _sp = ServiceProvider;
            _vm = VM;
            DataContext = VM;
            Title = TankSim.Constants.GameName;
            _ = this.BindDataContext(x => x.Content, (MainFormVM x) => x.FrameContent);
            InitializeComponent();
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            _vmInitTask = _vm.InitializeAsync();
        }

        protected override async void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            await _vmInitTask;

            //get game ID
            //build ardClient
            var gameScopeCtrl = _sp.GetRequiredService<GameScopeCtrl>();
            var gameScopeVM = _sp.GetRequiredService<GameScopeVM>();
            _vm.FrameContent = gameScopeCtrl;
            _scope = await gameScopeVM.IdTaskSource.Task;

        }


        protected override void Dispose(bool disposing)
        {
            _scope?.Dispose();
            base.Dispose(disposing);
        }

    }
}
