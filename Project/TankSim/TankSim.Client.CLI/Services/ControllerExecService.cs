using ArdNet.Client;
using ArdNet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.Extensions;
using TankSim.Client.OperatorModules;
using TankSim.Client.Services;
using TIPC.Core.Tools.Extensions;

namespace TankSim.Client.CLI.Services
{
    public class ControllerExecService : IDisposable
    {
        readonly IArdNetClient _ardClient;
        readonly IRoleResolverService _roleService;
        readonly IOperatorInputProcessorService _opInputService;

        public ControllerExecService(
            IArdNetClient ArdClient,
            IRoleResolverService RoleService,
            IOperatorInputProcessorService OpInputService)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            _ardClient = ArdClient;
            _roleService = RoleService;
            _opInputService = OpInputService;
        }

        public async Task<OperatorRoles> LoadOperatorRoles()
        {
            var opInitTask = _opInputService.Initialize();
            var roleSet = await _roleService.GetRolesAsync();
            await opInitTask;
            return roleSet;
        }

        public async Task SendUsername(string Username)
        {
            _ = await _ardClient.SendTcpCommandAsync(Constants.Commands.ControllerInit.SetClientName, Username);
        }

        public void HandleUserInput()
        {
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);

            while (true)
            {
                var key = Console.ReadKey();
                var arg = new OperatorInputEventArg(key, KeyInputType.KeyPress);
                var msg = new OperatorInputMsg(this, arg);
                _ardClient.MessageHub.EnqueueMessage(msg);

                Console.SetCursorPosition(CursorLeft, CursorTop);
            }
        }

        public void Dispose()
        {

        }
    }
}
