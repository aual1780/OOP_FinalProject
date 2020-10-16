using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI.OperatorModules
{
    [OperatorRole(OperatorRoles.RangeFinder)]
    public sealed class CliRangeFinder : CliModuleBase
    {
        readonly RangeFinderDelegate _ardDelegate;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        RangeDirection _currDirection;

        public CliRangeFinder(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }

            _ardDelegate = new RangeFinderDelegate(ArdClient);
            _keyBinding = KeyBinding;
            _currDirection = RangeDirection.Stop;
        }


        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.RangeFinder;
            //farther
            if (ValidateKeyPress(Input, keyConfig.Farther))
            {
                if (_currDirection == RangeDirection.Farther)
                {
                    _currDirection = RangeDirection.Stop;
                    _ardDelegate.Stop();
                }
                else
                {
                    _currDirection = RangeDirection.Farther;
                    _ardDelegate.AimFarther();
                }

                Input.IsHandled = true;
            }
            //closer
            if (ValidateKeyPress(Input, keyConfig.Closer))
            {
                if (_currDirection == RangeDirection.Closer)
                {
                    _currDirection = RangeDirection.Stop;
                    _ardDelegate.Stop();
                }
                else
                {
                    _currDirection = RangeDirection.Closer;
                    _ardDelegate.AimCloser();
                }

                Input.IsHandled = true;
            }
        }


        public override void Dispose()
        {
            _ardDelegate.Dispose();
        }
    }
}
