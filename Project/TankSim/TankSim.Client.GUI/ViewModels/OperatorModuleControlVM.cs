using ArdNet;
using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TIPC.Core.ComponentModel;

namespace TankSim.Client.GUI.ViewModels
{
    public class OperatorModuleControlVM : ViewModelBase
    {
        private readonly IArdNetClient _ardClient;
        private OperatorRoles _roles = 0;
        private IConnectedSystemEndpoint _gameHost;

        public OperatorRoles Roles
        {
            get => _roles;
            set => SetField(ref _roles, value);
        }
        public IConnectedSystemEndpoint GameHost
        {
            get => _gameHost;
            set => SetField(ref _gameHost, value);
        }

        public TaskCompletionSource<object> OpControllerTaskSource { get; } = new TaskCompletionSource<object>();

        public OperatorModuleControlVM(IArdNetClient ArdClient)
        {
            _ardClient = ArdClient;
        }

        public override async Task InitializeAsync()
        {
            GameHost = await _ardClient.ConnectAsync();
            var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
            var request = new AsyncRequestPushedArgs(qry, null, CancellationToken.None, Timeout.InfiniteTimeSpan);
            var response = await _ardClient.SendTcpQueryAsync(request);
            var responseStr = response.Single().Response;
            Roles = Enum.Parse<OperatorRoles>(responseStr);
        }
    }
}
