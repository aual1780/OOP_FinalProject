using ArdNet.Client;
using System;
using TankSim.Client.Config;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.OperatorModules
{
    public class CliDriver : IOperatorModule
    {
        readonly DriverDelegate _driver;
        readonly KeyBindingConfig _keyBinding;
        readonly DriveDirection _currDirection;

        public CliDriver(IArdNetClient ArdClient, KeyBindingConfig KeyBinding)
        {
            if (ArdClient is null)
            {
                throw new ArgumentNullException(nameof(ArdClient));
            }

            if (KeyBinding is null)
            {
                throw new ArgumentNullException(nameof(KeyBinding));
            }

            _driver = new DriverDelegate(ArdClient);
            _keyBinding = KeyBinding;
            _currDirection = DriveDirection.Stop;
        }


        public void HandleInput(IOperatorInputMsg Input)
        {
            if (string.Equals(_keyBinding.Driver.Forward, Input.KeyInfo.KeyChar.ToString()))
            {
                if (_currDirection == DriveDirection.Forward)
                    _driver.Stop();
                else
                    _driver.DriveForward();

                Input.IsHandled = true;
            }
            else if (string.Equals(_keyBinding.Driver.Backward, Input.KeyInfo.KeyChar.ToString()))
            {
                if (_currDirection == DriveDirection.Backward)
                    _driver.Stop();
                else
                    _driver.DriveBackward();

                Input.IsHandled = true;
            }
        }


        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
