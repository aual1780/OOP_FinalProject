using ArdNet.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.DependencyInjection;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.Uno.Frames.GameScope
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
            IServiceScope scope = null;
            var disposeScope = true;

            try
            {
                scope = _sp.CreateScope();
                var idService = scope.ServiceProvider.GetRequiredService<GameIdService>();
                idService.GameID = GameID;
                var ardClient = scope.ServiceProvider.GetRequiredService<IArdNetClient>();

                try
                {
                    using (var tokenSrc = new CancellationTokenSource(ConnectionTimeout))
                    {
                        var endptTask = ardClient.ConnectAsync(tokenSrc.Token);
                        var endpt = await endptTask;
                        if (endpt != null)
                        {
                            disposeScope = false;
                            StatusMsg = "Connected.";
                            return scope;
                        }
                        else
                        {
                            StatusMsg = "Cannot connect to the target host.";
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //noop
                    //continue search
                    StatusMsg = "Cannot connect to the target host.";
                }

                return null;
            }
            finally
            {
                if (disposeScope)
                {
                    scope?.Dispose();
                }
            }

        }
    }
}
