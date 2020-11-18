using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.OperatorDelegates;
using TankSim.Config;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TankSim.Client.OperatorModules
{
    [OperatorRole(OperatorRoles.Driver)]
    public sealed class DriverModule : OperatorInputModuleBase
    {
        private readonly DriverDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        private DriveDirection _currDirection = DriveDirection.Stop;

        public DriverModule(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
            _cmdDelegate = new DriverDelegate(ArdClient);
        }

        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.Driver;
            //forward
            if (ValidateKeyPress(Input, keyConfig.Forward))
            {
                if (Input.InputType == KeyInputType.KeyPress)
                {
                    if (_currDirection == DriveDirection.Forward)
                    {
                        _currDirection = DriveDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                    else
                    {
                        _currDirection = DriveDirection.Forward;
                        _cmdDelegate.DriveForward();
                    }
                }
                else if (Input.InputType == KeyInputType.KeyDown)
                {
                    _currDirection = DriveDirection.Forward;
                    _cmdDelegate.DriveForward();
                }
                else
                {
                    if (_currDirection == DriveDirection.Forward)
                    {
                        _currDirection = DriveDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                }

                Input.IsHandled = true;
            }
            //back
            else if (ValidateKeyPress(Input, keyConfig.Backward))
            {
                if (Input.InputType == KeyInputType.KeyPress)
                {
                    if (_currDirection == DriveDirection.Backward)
                    {
                        _currDirection = DriveDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                    else
                    {
                        _currDirection = DriveDirection.Backward;
                        _cmdDelegate.DriveBackward();
                    }
                }
                else if (Input.InputType == KeyInputType.KeyDown)
                {
                    _currDirection = DriveDirection.Backward;
                    _cmdDelegate.DriveBackward();
                }
                else
                {
                    if (_currDirection == DriveDirection.Backward)
                    {
                        _currDirection = DriveDirection.Stop;
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