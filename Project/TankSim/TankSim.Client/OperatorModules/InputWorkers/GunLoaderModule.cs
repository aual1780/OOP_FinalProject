using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Config;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TankSim.Client.OperatorModules
{
    [OperatorRole(OperatorRoles.GunLoader)]
    public sealed class GunLoaderModule : OperatorModuleBase
    {
        private readonly GunLoaderDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;

        public GunLoaderModule(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member