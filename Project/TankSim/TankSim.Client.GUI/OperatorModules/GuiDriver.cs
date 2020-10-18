using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.GUI.OperatorModules
{
    public sealed class GuiDriver : OperatorModuleBase
    {
        private readonly DriverDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        private DriveDirection _currDirection = DriveDirection.Stop;

        public GuiDriver(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
                if (Input.InputType == KeyInputType.KeyDown)
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
                if (Input.InputType == KeyInputType.KeyDown)
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
