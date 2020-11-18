using ArdNet.Client;
using System.Linq;
using System.Threading.Tasks;
using TIPC.Core.ComponentModel;
using TIPC.Core.Tools.Extensions;

namespace TankSim.Client.GUI.Frames.ClientName
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
                var procVal = value.Trim();
                var trueLen = procVal.GraphemeClusters().Count();
                if (trueLen <= _maxNameLength)
                {
                    _ = SetField(ref _username, procVal);
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

        public async Task SubmitName()
        {
            _ = await ArdClient.SendTcpCommandAsync(Constants.Commands.ControllerInit.SetClientName, Username);
            _ = NameTaskSource.TrySetResult(null);
        }
    }
}
