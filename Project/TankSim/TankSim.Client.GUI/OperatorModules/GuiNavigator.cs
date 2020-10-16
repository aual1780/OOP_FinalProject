using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.GUI.OperatorModules
{
    [OperatorRole(OperatorRoles.Navigator)]
    public sealed class GuiNavigator : GuiModuleBase
    {
        private readonly NavigatorDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        private RotationDirection _currDirection = RotationDirection.Stop;

        public GuiNavigator(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
            _cmdDelegate = new NavigatorDelegate(ArdClient);
        }

        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.Navigator;
            //left
            if (ValidateKeyPress(Input, keyConfig.Left))
            {
                if (Input.InputType == KeyInputType.KeyDown)
                {
                    _currDirection = RotationDirection.Left;
                    _cmdDelegate.TurnLeft();
                }
                else
                {
                    if (_currDirection == RotationDirection.Left)
                    {
                        _currDirection = RotationDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                }

                Input.IsHandled = true;
            }
            //right
            else if (ValidateKeyPress(Input, keyConfig.Right))
            {
                if (Input.InputType == KeyInputType.KeyDown)
                {
                    _currDirection = RotationDirection.Right;
                    _cmdDelegate.TurnRight();
                }
                else
                {
                    if (_currDirection == RotationDirection.Right)
                    {
                        _currDirection = RotationDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                }

                Input.IsHandled = true;
            }
        }

        public override void Dispose()
        {
            _cmdDelegate.Dispose();
        }
    }
}
