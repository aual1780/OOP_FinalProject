using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using TankSim.Client.GUI.Frames.ClientName;
using TankSim.Client.GUI.Frames.GameScope;
using TankSim.Client.GUI.Frames.Operations;

namespace TankSim.Client.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        readonly IServiceProvider _sp;
        readonly MainWindowVM _vm;
        Task _vmInitTask;
        IServiceScope _scope;

        public MainWindow(IServiceProvider ServiceProvider, MainWindowVM vm)
        {
            _sp = ServiceProvider;
            _vm = vm;
            this.Loaded += MainWindow_Loaded;

            this.DataContext = _vm;
            InitializeComponent();
        }

        public override void BeginInit()
        {
            base.BeginInit();
            _vmInitTask = _vm.InitializeAsync();
        }

        public override async void EndInit()
        {
            base.EndInit();
            await _vmInitTask;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //get game ID
            //build ardClient
            var gameScopeCtrl = _sp.GetRequiredService<GameScopeControl>();
            var gameScopeVM = _sp.GetRequiredService<GameScopeControlVM>();
            _vm.FrameContent = gameScopeCtrl;
            _scope = await gameScopeVM.IdTaskSource.Task;
            //start loading roles and dynamic UI modules in background
            var opModuleCtrl = _scope.ServiceProvider.GetRequiredService<OperatorModuleControl>();
            opModuleCtrl.BeginVmInit();
            //get username
            var clientNameCtrl = _scope.ServiceProvider.GetRequiredService<ClientNameControl>();
            var clientNameVM = _scope.ServiceProvider.GetRequiredService<ClientNameControlVM>();
            _vm.FrameContent = clientNameCtrl;
            _ = await clientNameVM.NameTaskSource.Task;
            //load operator module frame
            _vm.FrameContent = opModuleCtrl;
        }

        /// <summary>
        /// Dispose internal game scope
        /// </summary>
        public void Dispose()
        {
            _scope?.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
