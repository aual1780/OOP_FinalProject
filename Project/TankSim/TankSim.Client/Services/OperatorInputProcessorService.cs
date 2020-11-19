using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using TankSim.Client.Extensions;
using TankSim.Client.OperatorModules;
using TIPC.Core.Channels;
using TIPC.Core.Tools;
using TIPC.Core.Tools.Extensions;

namespace TankSim.Client.Services
{
    /// <summary>
    /// Operator input processor
    /// </summary>
    public interface IOperatorInputProcessorService
    {
        /// <summary>
        /// Initialize service
        /// </summary>
        /// <returns></returns>
        Task Initialize();
    }

    /// <summary>
    /// Operator input processor
    /// </summary>
    public class OperatorInputProcessorService :
        ThreadedMessageHubClient,
        IOperatorInputProcessorService
    {
        private readonly IRoleResolverService _roleService;
        private readonly IOperatorModuleFactory<IOperatorInputModule> _inputModuleFactory;
        private IEnumerable<IOperatorInputModule> _inputModuleCollection;

        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="MsgHub"></param>
        /// <param name="RoleService"></param>
        /// <param name="InputModuleFactory"></param>
        public OperatorInputProcessorService(
            IMessageHub MsgHub,
            IRoleResolverService RoleService,
            IOperatorModuleFactory<IOperatorInputModule> InputModuleFactory)
            : base(MsgHub, MaxThreadCount: 1)
        {
            _roleService = RoleService;
            _inputModuleFactory = InputModuleFactory;
            RegisterMessageProcessor<OperatorInputMsg>(OperatorInputProcessor);
        }

        /// <summary>
        /// Msg sub list
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ISafeType<IChannelMessage>> GetMessageSubscriptionList()
        {
            yield break;
        }

        /// <summary>
        /// Get roles 
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            var roles = await _roleService.GetRolesAsync();
            _inputModuleCollection = _inputModuleFactory.GetModuleCollection(roles);
            this.Start();
        }

        private void OperatorInputProcessor(Type t, OperatorInputMsg msg)
        {
            _inputModuleCollection?.SendInput(msg.Arg);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _inputModuleCollection?.DisposeAll();
            GC.SuppressFinalize(this);
        }
    }
}
