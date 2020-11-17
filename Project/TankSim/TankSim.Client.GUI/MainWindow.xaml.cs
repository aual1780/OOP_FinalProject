using Microsoft.Extensions.DependencyInjection;
using System;
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
    public partial class MainWindow : Window
    {
        readonly IServiceProvider _sp;
        readonly MainWindowVM _vm;
        IServiceScope _scope;

        public MainWindow(IServiceProvider ServiceProvider, MainWindowVM vm)
        {
            _sp = ServiceProvider;
            _vm = vm;
            this.Closing += MainWindow_Closing;
            this.Initialized += MainWindow_Initialized;
            this.Loaded += MainWindow_Loaded;

            this.DataContext = _vm;
            InitializeComponent();
        }

        private async void MainWindow_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //get game ID
            //build ardClient
            var gameScopeCtrl = _sp.GetRequiredService<GameScopeControl>();
            _vm.FrameContent = gameScopeCtrl;
            _scope = await gameScopeCtrl.GetGameScopeAsync();
            //start loading roles and dynamic UI modules in background
            var opModuleCtrl = _scope.ServiceProvider.GetRequiredService<OperatorModuleControl>();
            opModuleCtrl.BeginVmInit();
            //get username
            var clientNameCtrl = _scope.ServiceProvider.GetRequiredService<ClientNameControl>();
            _vm.FrameContent = clientNameCtrl;
            await clientNameCtrl.SendClientNameAsync();
            //load operator module frame
            _vm.FrameContent = opModuleCtrl;
        }


        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await Task.Run(() =>
            {
                _scope?.Dispose();
            });
        }
    }
}
