using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.OperatorDelegates;
using TankSim.Config;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TankSim.Client.OperatorModules
{
    [OperatorRole(OperatorRoles.FireControl)]
    public sealed class FireControlModule : OperatorInputModuleBase
    {
        private readonly FireControlDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public FireControlModule(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }
            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }
            _keyBinding = KeyBinding;
            _cmdDelegate = new FireControlDelegate(ArdClient);
        }

        public override void HandleInput(IOperatorInputMsg Input)
        {
            if (Input.InputType == KeyInputType.KeyUp)
                return;

            var keyConfig = _keyBinding.CurrentValue.FireControl;
            //fire primary
            if (ValidateKeyPress(Input, keyConfig.Primary))
            {
                _cmdDelegate.FirePrimary();
                Input.IsHandled = true;
            }
            //fire secondary
            else if (ValidateKeyPress(Input, keyConfig.Secondary))
            {
                _cmdDelegate.FireSecondary();
                Input.IsHandled = true;
            }
        }

        public override void Dispose()
        {
            _cmdDelegate.Dispose();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member