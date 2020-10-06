using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.CLI.OperatorModules
{
    public class CliNavigator : IOperatorModule
    {
        readonly NavigatorDelegate _navigator;
        readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        AngleDirection _currDirection;

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

            _navigator = new NavigatorDelegate(ArdClient);
            _keyBinding = KeyBinding;
            _currDirection = AngleDirection.Stop;
        }


        public void HandleInput(IOperatorInputMsg Input)
        {
            var navigatorConfig = _keyBinding.CurrentValue.Navigator;
            //left
            if (string.Equals(navigatorConfig.Left, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (_currDirection == AngleDirection.Left)
                {
                    _currDirection = AngleDirection.Stop;
                    _navigator.Stop();
                }
                else
                {
                    _currDirection = AngleDirection.Left;
                    _navigator.TurnLeft();
                }

                Input.IsHandled = true;
            }
            //right
            else if (string.Equals(navigatorConfig.Right, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (_currDirection == AngleDirection.Right)
                {
                    _currDirection = AngleDirection.Stop;
                    _navigator.Stop();
                }
                else
                {
                    _currDirection = AngleDirection.Right;
                    _navigator.TurnRight();
                }

                Input.IsHandled = true;
            }
        }


        public void Dispose()
        {
            _navigator.Dispose();
        }
    }
}
