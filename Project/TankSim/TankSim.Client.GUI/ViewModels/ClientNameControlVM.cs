using ArdNet.Client;
using System.Threading.Tasks;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.ViewModels
{
    public class ClientNameControlVM : ViewModelBase
    {
        private readonly int _maxNameLength = 20;
        private string _username;
        private string _statusMsg;
        private bool _isUIEnabled = true;
        private readonly TaskCompletionSource<object> _nameTaskSource;

        public IArdNetClient ArdClient { get; }
        public string Username
        {
            get => _username;
            set
            {
                if (value.Length < _maxNameLength)
                {
                    _ = SetField(ref _username, value);
                }
            }
        }
        public string StatusMsg
        {
            get => _statusMsg;
            set => SetField(ref _statusMsg, value);
        }
        public bool IsUIEnabled
        {
            get => _isUIEnabled;
            set => SetField(ref _isUIEnabled, value);
        }
        public TaskCompletionSource<object> NameTaskSource
        {
            get => _nameTaskSource;
        }




        public ClientNameControlVM(IArdNetClient ArdClient)
        {
            this.ArdClient = ArdClient;
            _nameTaskSource = new TaskCompletionSource<object>();
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
