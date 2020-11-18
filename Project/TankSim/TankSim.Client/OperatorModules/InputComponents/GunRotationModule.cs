using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.OperatorDelegates;
using TankSim.Config;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TankSim.Client.OperatorModules
{
    [OperatorRole(OperatorRoles.GunRotation)]
    public sealed class GunRotationModule : OperatorInputModuleBase
    {
        private readonly GunRotationDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        private RotationDirection _currDirection = RotationDirection.Stop;

        public GunRotationModule(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
            _cmdDelegate = new GunRotationDelegate(ArdClient);
        }

        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.GunRotation;
            //left
            if (ValidateKeyPress(Input, keyConfig.Left))
            {
                if (Input.InputType == KeyInputType.KeyPress)
                {
                    if (_currDirection == RotationDirection.Left)
                    {
                        _currDirection = RotationDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                    else
                    {
                        _currDirection = RotationDirection.Left;
                        _cmdDelegate.TurnLeft();
                    }
                }
                else if (Input.InputType == KeyInputType.KeyDown)
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
                if (Input.InputType == KeyInputType.KeyPress)
                {
                    if (_currDirection == RotationDirection.Right)
                    {
                        _currDirection = RotationDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                    else
                    {
                        _currDirection = RotationDirection.Right;
                        _cmdDelegate.TurnRight();
                    }
                }
                else if (Input.InputType == KeyInputType.KeyDown)
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member