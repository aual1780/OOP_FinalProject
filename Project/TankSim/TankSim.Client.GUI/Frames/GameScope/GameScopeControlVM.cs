using ArdNet.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.Services;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.Frames.GameScope
{
    public class GameScopeControlVM : ViewModelBase
    {
        readonly IServiceProvider _sp;
        private string _gameID;
        private string _errorMsg;
        private bool _isUIEnabled = true;

        public string GameID
        {
            get => _gameID;
            set
            {
                _ = SetField(ref _gameID, value);
            }
        }

        public string StatusMsg
        {
            get => _errorMsg;
            set => SetField(ref _errorMsg, value);
        }

        public bool IsUIEnabled
        {
            get => _isUIEnabled;
            set => SetField(ref _isUIEnabled, value);
        }

        public TaskCompletionSource<IServiceScope> IdTaskSource
        {
            get;
        }

        public TimeSpan ConnectionTimeout { get; } = TimeSpan.FromSeconds(3);

        public GameScopeControlVM(IServiceProvider Provider)
        {
            _sp = Provider;
            IdTaskSource = new TaskCompletionSource<IServiceScope>();
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<IServiceScope> ValidateGameID()
        {
            var canConnect = false;

            var scope = _sp.CreateScope();
            var idService = scope.ServiceProvider.GetRequiredService<GameIdService>();
            idService.GameID = GameID;
            var ardClient = scope.ServiceProvider.GetRequiredService<IArdNetClient>();

            using (var tokenSrc = new CancellationTokenSource(ConnectionTimeout))
            {
                try
                {
                    var endptTask = ardClient.ConnectAsync(tokenSrc.Token);
                    var endpt = await endptTask;
                    if (endpt != null && !tokenSrc.IsCancellationRequested)
                    {
                        StatusMsg = "Connected.";
                        return scope;
                    }
                    else
                    {
                        StatusMsg = "Cannot connect to the target host.";
                    }
                }
                catch (OperationCanceledException)
                {
                    //noop
                    //continue search
                    StatusMsg = "Cannot connect to the target host.";
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
    }
}
