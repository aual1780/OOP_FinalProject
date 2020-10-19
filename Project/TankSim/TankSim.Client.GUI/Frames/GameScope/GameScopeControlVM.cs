using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.Frames.GameScope
{
    public class GameScopeControlVM : ViewModelBase
    {
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

        public GameScopeControlVM()
        {
            IdTaskSource = new TaskCompletionSource<IServiceScope>();
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
