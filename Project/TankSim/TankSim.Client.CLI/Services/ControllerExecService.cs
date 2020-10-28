using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.Extensions;
using TankSim.Client.OperatorModules;
using TIPC.Core.Tools.Extensions.IEnumerable;

namespace TankSim.Client.CLI.Services
{
    public class ControllerExecService : IDisposable
    {
        IEnumerable<IOperatorInputModule> _opCollection;
        readonly IArdNetClient _ardClient;
        readonly IOperatorModuleFactory<IOperatorInputModule> _operatorFactory;
        readonly CancellationTokenSource _tokenSrc = new CancellationTokenSource();

        public ControllerExecService(IArdNetClient ArdClient, IOperatorModuleFactory<IOperatorInputModule> OperatorFactory)
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
            var request = new AsyncRequestPushedArgs(qry, null, _tokenSrc.Token, Timeout.InfiniteTimeSpan);
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
            try
            {
                _tokenSrc.Cancel();
                _tokenSrc.Dispose();
            }
            catch
            {
                //noop
            }
            _opCollection?.DisposeAll();
        }
    }
}
