using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.Client.OperatorDelegates;
using TankSim.Client.OperatorModules;
using TankSim.Config;

namespace TankSim.Client.GUI.OperatorModules
{
    public sealed class GuiRangeFinder : GuiModuleBase
    {
        private readonly RangeFinderDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        private RangeDirection _currDirection = RangeDirection.Stop;

        public GuiRangeFinder(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
            _cmdDelegate = new RangeFinderDelegate(ArdClient);
        }

        public override void HandleInput(IOperatorInputMsg Input)
        {
            var keyConfig = _keyBinding.CurrentValue.RangeFinder;
            //farther
            if (ValidateKeyPress(Input, keyConfig.Farther))
            {
                if (Input.InputType == KeyInputType.KeyDown)
                {
                    _currDirection = RangeDirection.Farther;
                    _cmdDelegate.AimFarther();
                }
                else
                {
                    if (_currDirection == RangeDirection.Farther)
                    {
                        _currDirection = RangeDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                }

                Input.IsHandled = true;
            }
            //closer
            else if (ValidateKeyPress(Input, keyConfig.Closer))
            {
                if (Input.InputType == KeyInputType.KeyDown)
                {
                    _currDirection = RangeDirection.Closer;
                    _cmdDelegate.AimCloser();
                }
                else
                {
                    if (_currDirection == RangeDirection.Closer)
                    {
                        _currDirection = RangeDirection.Stop;
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
