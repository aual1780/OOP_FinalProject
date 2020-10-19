using ArdNet.Client;
using Microsoft.Extensions.Options;
using System;
using TankSim.OperatorDelegates;
using TankSim.Config;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TankSim.Client.OperatorModules
{
    [OperatorRole(OperatorRoles.RangeFinder)]
    public sealed class RangeFinderModule : OperatorModuleBase
    {
        private readonly RangeFinderDelegate _cmdDelegate;
        private readonly IOptionsMonitor<KeyBindingConfig> _keyBinding;
        private RangeDirection _currDirection = RangeDirection.Stop;

        public RangeFinderModule(IArdNetClient ArdClient, IOptionsMonitor<KeyBindingConfig> KeyBinding)
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
                if (Input.InputType == KeyInputType.KeyPress)
                {
                    if (_currDirection == RangeDirection.Farther)
                    {
                        _currDirection = RangeDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                    else
                    {
                        _currDirection = RangeDirection.Farther;
                        _cmdDelegate.AimFarther();
                    }
                }
                else if (Input.InputType == KeyInputType.KeyDown)
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
                if (Input.InputType == KeyInputType.KeyPress)
                {
                    if (_currDirection == RangeDirection.Closer)
                    {
                        _currDirection = RangeDirection.Stop;
                        _cmdDelegate.Stop();
                    }
                    else
                    {
                        _currDirection = RangeDirection.Closer;
                        _cmdDelegate.AimCloser();
                    }
                }
                else if (Input.InputType == KeyInputType.KeyDown)
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member