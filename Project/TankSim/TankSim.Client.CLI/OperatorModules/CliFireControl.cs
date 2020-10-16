using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI.OperatorModules
{
    [OperatorRole(OperatorRoles.FireControl)]
    public sealed class CliFireControl : CliModuleBase
    {
        readonly FireControlDelegate _ardDelegate;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public CliFireControl(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }

            _ardDelegate = new FireControlDelegate(ArdClient);
            _keyBinding = KeyBinding;
        }


        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.FireControl;
            //primary
            if (ValidateKeyPress(Input, keyConfig.Primary))
            {
                _ardDelegate.FirePrimary();
                Input.IsHandled = true;
            }
            //secondary
            if (ValidateKeyPress(Input, keyConfig.Secondary))
            {
                _ardDelegate.FireSecondary();
                Input.IsHandled = true;
            }
        }


        public override void Dispose()
        {
            _ardDelegate.Dispose();
        }
    }
}
