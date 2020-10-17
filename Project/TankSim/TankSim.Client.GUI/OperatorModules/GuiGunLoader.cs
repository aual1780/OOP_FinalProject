using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.GUI.OperatorModules
{
    [OperatorRole(OperatorRoles.GunLoader)]
    public sealed class GuiGunLoader : OperatorModuleBase
    {
        private readonly GunLoaderDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public GuiGunLoader(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
            _cmdDelegate = new GunLoaderDelegate(ArdClient);
        }

        public override void HandleInput(IOperatorInputMsg Input)
        {
            if (Input.InputType == KeyInputType.KeyUp)
                return;

            var keyConfig = _keyBinding.CurrentValue.GunLoader;
            //fire primary
            if (ValidateKeyPress(Input, keyConfig.Load))
            {
                _cmdDelegate.Load();
                Input.IsHandled = true;
            }
            //fire secondary
            else if (ValidateKeyPress(Input, keyConfig.CycleAmmo))
            {
                _cmdDelegate.CycleAmmoType();
                Input.IsHandled = true;
            }
        }

        public override void Dispose()
        {
            _cmdDelegate.Dispose();
        }
    }
}
