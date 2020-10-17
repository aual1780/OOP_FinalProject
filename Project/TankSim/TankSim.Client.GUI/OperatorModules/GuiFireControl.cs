using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.GUI.OperatorModules
{
    [OperatorRole(OperatorRoles.FireControl)]
    public sealed class GuiFireControl : OperatorModuleBase
    {
        private readonly FireControlDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public GuiFireControl(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
