using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI.OperatorModules
{
    [OperatorRole(OperatorRoles.GunLoader)]
    public sealed class CliGunLoader : CliModuleBase
    {
        readonly GunLoaderDelegate _ardDelegate;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public CliGunLoader(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }

            _ardDelegate = new GunLoaderDelegate(ArdClient);
            _keyBinding = KeyBinding;
        }


        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.GunLoader;
            //load
            if (ValidateKeyPress(Input, keyConfig.Load))
            {
                _ardDelegate.Load();
                Input.IsHandled = true;
            }
            //cycle ammo
            if (ValidateKeyPress(Input, keyConfig.CycleAmmo))
            {
                _ardDelegate.CycleAmmoType();
                Input.IsHandled = true;
            }
        }


        public override void Dispose()
        {
            _ardDelegate.Dispose();
        }
    }
}
