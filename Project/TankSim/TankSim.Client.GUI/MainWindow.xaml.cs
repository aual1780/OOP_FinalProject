using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TankSim.Client.GUI.Controls;
using TankSim.Client.GUI.ViewModels;

namespace TankSim.Client.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IServiceProvider _sp;
        readonly MainWindowVM _vm;

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

        private void MainWindow_Initialized(object sender, EventArgs e)
        {

        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //get game ID
            //build ardClient
            var gameScopeCtrl = _sp.GetRequiredService<GameScopeControl>();
            _vm.FrameContent = gameScopeCtrl;
            using var scope = await gameScopeCtrl.GetGameScopeAsync();
            //get username
            var clientNameCtrl = scope.ServiceProvider.GetRequiredService<ClientNameControl>();
            _vm.FrameContent = clientNameCtrl;
            await clientNameCtrl.SendClientNameAsync();
            //load operator module frame
            var opModuleCtrl = scope.ServiceProvider.GetRequiredService<OperatorModuleControl>();
            _vm.FrameContent = opModuleCtrl;
            await opModuleCtrl.ControllerExecAsync();
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
