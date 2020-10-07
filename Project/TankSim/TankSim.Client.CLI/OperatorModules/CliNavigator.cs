using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI.OperatorModules
{
    public sealed class CliNavigator : CliModuleBase
    {
        readonly NavigatorDelegate _ardDelegate;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        RotationDirection _currDirection;

        public CliNavigator(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }

            _ardDelegate = new NavigatorDelegate(ArdClient);
            _keyBinding = KeyBinding;
            _currDirection = RotationDirection.Stop;
        }


        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.Navigator;
            //left
            if (ValidateKeyPress(Input, keyConfig.Left))
            {
                if (_currDirection == RotationDirection.Left)
                {
                    _currDirection = RotationDirection.Stop;
                    _ardDelegate.Stop();
                }
                else
                {
                    _currDirection = RotationDirection.Left;
                    _ardDelegate.TurnLeft();
                }

                Input.IsHandled = true;
            }
            //right
            if (ValidateKeyPress(Input, keyConfig.Right))
            {
                if (_currDirection == RotationDirection.Right)
                {
                    _currDirection = RotationDirection.Stop;
                    _ardDelegate.Stop();
                }
                else
                {
                    _currDirection = RotationDirection.Right;
                    _ardDelegate.TurnRight();
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
