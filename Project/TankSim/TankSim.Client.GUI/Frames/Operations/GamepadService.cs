using ArdNet;
using ArdNet.Client;
using J2i.Net.XInputWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using TankSim.OperatorDelegates;
using TIPC.Core.Tools;
using TIPC.Core.Tools.Extensions.IEnumerable;

namespace TankSim.Client.GUI.Frames.Operations
{
    public interface IGamepadService
    {
        int GamepadIndex { get; set; }
        bool TrySetControllerIndex(int Idx);
        void SetRoles(OperatorRoles Roles);
    }

    public class GamepadService : IGamepadService, IDisposable
    {
        private readonly IArdNetClient _ardClient;
        private int _gamepadIndex = -1;
        private readonly object _controllerLock = new object();
        private XboxController _controller;
        private OperatorRoles _roles;
        private readonly List<IDisposable> _operatorDelegates = new List<IDisposable>();
        private event EventHandler<XboxControllerStateChangedEventArgs> ControllerStateChanged;
        const int _thumbThreshhold = (int)(XboxController.ThumbStick.MAX_THUMBSTICK_VAL / 2.0);
        const int _triggerThreshhold = (int)(XboxController.Trigger.MAX_TRIGGER_VAL / 2.0);

        public int GamepadIndex
        {
            get => _gamepadIndex;
            set
            {
                if (_gamepadIndex == value)
                {
                    return;
                }
                lock (_controllerLock)
                {
                    _gamepadIndex = value;
                    if (_controller != null)
                    {
                        _controller.StateChanged -= Controller_StateChanged;
                    }
                    _controller = XboxController.RetrieveController(_gamepadIndex);
                    _controller.StateChanged += Controller_StateChanged;
                }
            }
        }

        public XboxController Controller => _controller;


        private void Controller_StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            ControllerStateChanged?.Invoke(sender, e);
        }

        public GamepadService(IArdNetClient ArdClient)
        {
            GamepadIndex = 0;
            _ardClient = ArdClient;
        }

        public bool TrySetControllerIndex(int Idx)
        {
            if (Idx < XboxController.FIRST_CONTROLLER_INDEX)
            {
                return false;
            }
            if (Idx > XboxController.LAST_CONTROLLER_INDEX)
            {
                return false;
            }
            GamepadIndex = Idx;
            return true;
        }

        public void SetRoles(OperatorRoles Roles)
        {
            this.Dispose();
            XboxController.StartPolling();
            _roles = Roles;
            var flags = EnumTools.GetSelectedFlags(Roles).ToHashSet();
            if (flags.Contains(OperatorRoles.Driver))
            {
                var proxy = new DriverDelegate(_ardClient);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.FireControl))
            {
                var proxy = new FireControlDelegate(_ardClient);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.GunLoader))
            {
                var proxy = new GunLoaderDelegate(_ardClient);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.GunRotation))
            {
                var proxy = new GunRotationDelegate(_ardClient);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.Navigator))
            {
                var proxy = new NavigatorDelegate(_ardClient);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.RangeFinder))
            {
                var proxy = new RangeFinderDelegate(_ardClient);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
        }

        private void AddOperatorHandler(DriverDelegate proxy)
        {
            ControllerStateChanged += (sender, arg) =>
            {
                var current = arg.CurrentInputState.Gamepad.sThumbLY;
                var absCurrent = Math.Abs((int)arg.CurrentInputState.Gamepad.sThumbLY);
                var absPrev = Math.Abs((int)arg.PreviousInputState.Gamepad.sThumbLY);

                if (
                current > 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.DriveForward();
                }
                else if (
                current < 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.DriveBackward();
                }
                else if (
                absCurrent < _thumbThreshhold &&
                absPrev > _thumbThreshhold)
                {
                    proxy.Stop();
                }
            };
        }

        private void AddOperatorHandler(FireControlDelegate proxy)
        {
            ControllerStateChanged += (sender, arg) =>
            {
                var currentR = (int)arg.CurrentInputState.Gamepad.bRightTrigger;
                var prevR = (int)arg.PreviousInputState.Gamepad.bRightTrigger;
                var currentL = (int)arg.CurrentInputState.Gamepad.bLeftTrigger;
                var prevL = (int)arg.PreviousInputState.Gamepad.bLeftTrigger;

                if (
                currentR > _triggerThreshhold &&
                prevR < _triggerThreshhold)
                {
                    proxy.FirePrimary();
                }
                if (
                currentL > _triggerThreshhold &&
                prevL < _triggerThreshhold)
                {
                    proxy.FireSecondary();
                }
            };
        }

        private void AddOperatorHandler(GunLoaderDelegate proxy)
        {
            ControllerStateChanged += (sender, arg) =>
            {
                var currentX = arg.CurrentInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_X);
                var prevX = arg.PreviousInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_X);
                var currentY = arg.CurrentInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_Y);
                var prevY = arg.PreviousInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_Y);

                if (currentX && !prevX)
                {
                    proxy.Load();
                }
                if (currentY && !prevY)
                {
                    proxy.CycleAmmoType();
                }
            };
        }

        private void AddOperatorHandler(GunRotationDelegate proxy)
        {
            ControllerStateChanged += (sender, arg) =>
            {
                var current = (int)arg.CurrentInputState.Gamepad.sThumbRX;
                var absCurrent = Math.Abs((int)arg.CurrentInputState.Gamepad.sThumbRX);
                var absPrev = Math.Abs((int)arg.PreviousInputState.Gamepad.sThumbRX);

                if (
                current > 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.TurnRight();
                }
                else if (
                current < 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.TurnLeft();
                }
                else if (
                absCurrent < _thumbThreshhold &&
                absPrev > _thumbThreshhold)
                {
                    proxy.Stop();
                }
            };
        }

        private void AddOperatorHandler(NavigatorDelegate proxy)
        {
            ControllerStateChanged += (sender, arg) =>
            {
                var current = (int)arg.CurrentInputState.Gamepad.sThumbLX;
                var absCurrent = Math.Abs((int)arg.CurrentInputState.Gamepad.sThumbLX);
                var absPrev = Math.Abs((int)arg.PreviousInputState.Gamepad.sThumbLX);

                if (
                current > 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.TurnRight();
                }
                else if (
                current < 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.TurnLeft();
                }
                else if (
                absCurrent < _thumbThreshhold &&
                absPrev > _thumbThreshhold)
                {
                    proxy.Stop();
                }
            };
        }

        private void AddOperatorHandler(RangeFinderDelegate proxy)
        {
            ControllerStateChanged += (sender, arg) =>
            {
                var current = arg.CurrentInputState.Gamepad.sThumbRY;
                var absCurrent = Math.Abs((int)arg.CurrentInputState.Gamepad.sThumbRY);
                var absPrev = Math.Abs((int)arg.PreviousInputState.Gamepad.sThumbRY);

                if (
                current > 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.AimFarther();
                }
                else if (
                current < 0 &&
                absCurrent > _thumbThreshhold &&
                absPrev < _thumbThreshhold)
                {
                    proxy.AimCloser();
                }
                else if (
                absCurrent < _thumbThreshhold &&
                absPrev > _thumbThreshhold)
                {
                    proxy.Stop();
                }
            };
        }

        public void Dispose()
        {
            ControllerStateChanged = null;
            _operatorDelegates.DisposeAll();
            _operatorDelegates.Clear();
            XboxController.StopPolling();
        }
    }
}
