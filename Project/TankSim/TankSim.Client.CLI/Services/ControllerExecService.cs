using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.CLI.OperatorModules;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.Services
{
    public class ControllerExecService : IDisposable
    {
        IOperatorModuleCollection _opCollection;
        readonly IArdNetClient _ardClient;
        readonly OperatorModuleFactory _operatorFactory;

        public ControllerExecService(IArdNetClient ArdClient, IOperatorModuleFactory OperatorFactory)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (OperatorFactory is null)
            {
                throw new ArgumentNullException(nameof(OperatorFactory));
            }

            _ardClient = ArdClient;
            _operatorFactory = OperatorFactory;
        }

        public async Task<OperatorRoles> LoadOperatorRoles()
        {
            var qry = Constants.Queries.ControllerInit.GetOperatorRoles;
            var request = new AsyncRequestPushedArgs(qry, null, CancellationToken.None, Timeout.InfiniteTimeSpan);
            var responseSet = await _ardClient.SendTcpQueryAsync(request);
            var responseStr = responseSet.Single().Response;
            var roleSet = Enum.Parse<OperatorRoles>(responseStr);

            _opCollection = _operatorFactory.GetModuleCollection(roleSet);
            return roleSet;
        }

        public void HandleUserInput()
        {
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);

            while (true)
            {
                var key = Console.ReadKey();
                var msg = new OperatorInputMsg(key, KeyInputType.KeyPress);
                _opCollection.SendInput(msg);
                Console.SetCursorPosition(CursorLeft, CursorTop);
            }

        }

        public void Dispose()
        {
            _opCollection?.Dispose();
        }
    }
}
