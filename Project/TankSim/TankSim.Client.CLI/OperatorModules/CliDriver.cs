using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.Config;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;

namespace TankSim.Client.CLI.OperatorModules
{
    public class CliDriver : IOperatorModule
    {
        readonly DriverDelegate _driver;
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

            _driver = new DriverDelegate(ArdClient);
            _keyBinding = KeyBinding;
            _currDirection = DriveDirection.Stop;
        }


        public void HandleInput(IOperatorInputMsg Input)
        {
            var driverConfig = _keyBinding.CurrentValue.Driver;
            if (string.Equals(driverConfig.Forward, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (_currDirection == DriveDirection.Forward)
                {
                    _currDirection = DriveDirection.Stop;
                    _driver.Stop();
                }
                else
                {
                    _currDirection = DriveDirection.Forward;
                    _driver.DriveForward();
                }

                Input.IsHandled = true;
            }
            else if (string.Equals(driverConfig.Backward, Input.KeyInfo.KeyChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (_currDirection == DriveDirection.Backward)
                {
                    _currDirection = DriveDirection.Stop;
                    _driver.Stop();
                }
                else
                {
                    _currDirection = DriveDirection.Backward;
                    _driver.DriveBackward();
                }

                Input.IsHandled = true;
            }
        }


        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
