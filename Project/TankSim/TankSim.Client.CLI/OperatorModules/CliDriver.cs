using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI.OperatorModules
{
    [OperatorRole(OperatorRoles.Driver)]
    public sealed class CliDriver : OperatorModuleBase
    {
        readonly DriverDelegate _ardDelegate;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        DriveDirection _currDirection;

        public CliDriver(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }

            _ardDelegate = new DriverDelegate(ArdClient);
            _keyBinding = KeyBinding;
            _currDirection = DriveDirection.Stop;
        }


        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.Driver;
            //forward
            if (ValidateKeyPress(Input, keyConfig.Forward))
            {
                if (_currDirection == DriveDirection.Forward)
                {
                    _currDirection = DriveDirection.Stop;
                    _ardDelegate.Stop();
                }
                else
                {
                    _currDirection = DriveDirection.Forward;
                    _ardDelegate.DriveForward();
                }

                Input.IsHandled = true;
            }
            //back
            else if (ValidateKeyPress(Input, keyConfig.Backward))
            {
                if (_currDirection == DriveDirection.Backward)
                {
                    _currDirection = DriveDirection.Stop;
                    _ardDelegate.Stop();
                }
                else
                {
                    _currDirection = DriveDirection.Backward;
                    _ardDelegate.DriveBackward();
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
