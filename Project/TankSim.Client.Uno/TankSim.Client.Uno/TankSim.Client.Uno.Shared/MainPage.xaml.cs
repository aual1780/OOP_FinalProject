using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TankSim.Client.GUI.Frames.GameScope;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TankSim.Client.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDisposable
    {
        readonly IServiceProvider _sp;
        readonly MainWindowVM _vm;
        Task _vmInitTask;
        IServiceScope _scope;

        public MainPage()
        {
            _sp = DiContainer.Instance();
            _vm = DiContainer.Instance().GetRequiredService<MainWindowVM>();
            this.Loading += MainPage_Loading;
            this.Loaded += MainWindow_Loaded;

            this.DataContext = _vm;
            InitializeComponent();
        }

        private void MainPage_Loading(FrameworkElement sender, object args)
        {
            _vmInitTask = _vm.InitializeAsync();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _vmInitTask;
            //TODO
            //get game ID
            //build ardClient
            var gameScopeCtrl = _sp.GetRequiredService<GameScopeControl>();
            _vm.FrameContent = gameScopeCtrl;
            _scope = await gameScopeCtrl.GetGameScopeAsync();
            ////start loading roles and dynamic UI modules in background
            //var opModuleCtrl = _scope.ServiceProvider.GetRequiredService<OperatorModuleControl>();
            //opModuleCtrl.BeginVmInit();
            ////get username
            //var clientNameCtrl = _scope.ServiceProvider.GetRequiredService<ClientNameControl>();
            //_vm.FrameContent = clientNameCtrl;
            //await clientNameCtrl.SendClientNameAsync();
            ////load operator module frame
            //_vm.FrameContent = opModuleCtrl;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
