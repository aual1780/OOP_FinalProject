﻿using ArdNet.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TankSim.Client.GUI.ViewModels;
using TankSim.Client.Services;

namespace TankSim.Client.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GameScopeControl.xaml
    /// </summary>
    public partial class GameScopeControl : UserControl
    {
        readonly IServiceProvider _sp;
        readonly GameScopeControlVM _vm;

        public GameScopeControl(IServiceProvider ServiceProvider, GameScopeControlVM vm)
        {
            this.Initialized += GameScopeControl_Initialized;
            _sp = ServiceProvider;
            _vm = vm;
            this.DataContext = _vm;
            InitializeComponent();
        }

        private async void GameScopeControl_Initialized(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
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

                Cursor = Cursors.Wait;
                var scope = await Task.Run(BtnClick);
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
                Cursor = Cursors.Arrow;
            }
        }

        private async Task<IServiceScope> BtnClick()
        {
            var canConnect = false;

            var scope = _sp.CreateScope();
            var idService = scope.ServiceProvider.GetRequiredService<GameIdService>();
            idService.GameID = _vm.GameID;
            var ardClient = scope.ServiceProvider.GetRequiredService<IArdNetClient>();

            using (var tokenSrc = new CancellationTokenSource(_vm.ConnectionTimeout))
            {
                try
                {
                    var endptTask = ardClient.ConnectAsync(tokenSrc.Token);
                    var endpt = await endptTask;
                    if (endpt != null && !tokenSrc.IsCancellationRequested)
                    {
                        _vm.StatusMsg = "Connected.";
                        return scope;
                    }
                    else
                    {
                        _vm.StatusMsg = "Cannot connect to the target host.";
                    }
                }
                catch (OperationCanceledException)
                {
                    //noop
                    //continue search
                    _vm.StatusMsg = "Cannot connect to the target host.";
                }

            }

            if (canConnect)
            {
                return scope;
            }
            else
            {
                scope.Dispose();
                return null;
            }

        }

        public Task<IServiceScope> GetGameScopeAsync()
        {
            return _vm.IdTaskSource.Task;
        }
    }
}
