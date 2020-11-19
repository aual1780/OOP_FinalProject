using ArdNet;
using J2i.Net.XInputWrapper;
using System;
using System.Collections.Generic;
using TankSim.OperatorDelegates;
using TIPC.Core.Tools;
using TIPC.Core.Tools.Extensions;

namespace TankSim
{
    /// <summary>
    /// Gamepad service interface
    /// </summary>
    public interface IGamepadService : IDisposable
    {
        /// <summary>
        /// Gamepad ID.  Lets user select gamepad to use
        /// </summary>
        int GamepadIndex { get; }
        /// <summary>
        /// Currently selected controller instance
        /// </summary>
        XboxController Gamepad { get; }
        /// <summary>
        /// Attempt to change the target controller.
        /// Must be in range 0-7
        /// </summary>
        /// <param name="Idx"></param>
        /// <returns></returns>
        bool TrySetControllerIndex(int Idx);
        /// <summary>
        /// Set operator roles.
        /// Configures bindings to hook proper controls for the assigned operator roles
        /// </summary>
        /// <param name="Roles"></param>
        void SetRoles(OperatorRoles Roles);
    }

    /// <summary>
    /// Gamepad binding service
    /// </summary>
    public class GamepadService : IGamepadService
    {
        private readonly IArdNetSystem _ardSys;
        private readonly XboxControllerManager _xboxManager;
        private readonly object _controllerLock = new();
        private readonly List<IDisposable> _operatorDelegates = new();
        private int _gamepadIndex = -1;
        private XboxController _controller;
        private event EventHandler<XboxControllerStateChangedEventArgs> ControllerStateChanged;
        const int _thumbThreshhold = (int)(XboxController.ThumbStick.MAX_THUMBSTICK_VAL / 2.0);
        const int _triggerThreshhold = (int)(XboxController.Trigger.MAX_TRIGGER_VAL / 2.0);

        /// <summary>
        /// Current gamepad ID
        /// </summary>
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
                    _controller = _xboxManager.RetrieveController(_gamepadIndex);
                    _controller.StateChanged += Controller_StateChanged;
                }
            }
        }

        /// <summary>
        /// Get current gamepad
        /// </summary>
        public XboxController Gamepad => _controller;


        private void Controller_StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            ControllerStateChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="ArdClient"></param>
        /// <param name="XboxManager"></param>
        public GamepadService(IArdNetSystem ArdClient, XboxControllerManager XboxManager)
        {
            _ardSys = ArdClient;
            _xboxManager = XboxManager;
            GamepadIndex = 0;
        }

        /// <summary>
        /// Attempt to change the target controller.
        /// Must be in range 0-7
        /// </summary>
        /// <param name="Idx"></param>
        /// <returns></returns>
        public bool TrySetControllerIndex(int Idx)
        {
            if (Idx < XboxControllerManager.FIRST_CONTROLLER_INDEX)
            {
                return false;
            }
            if (Idx > XboxControllerManager.LAST_CONTROLLER_INDEX)
            {
                return false;
            }
            GamepadIndex = Idx;
            return true;
        }

        /// <summary>
        /// Set operator roles.
        /// Configures bindings to hook proper controls for the assigned operator roles
        /// </summary>
        /// <param name="Roles"></param>
        public void SetRoles(OperatorRoles Roles)
        {
            this.Dispose();
            _xboxManager.StartPolling();
            var flags = new HashSet<OperatorRoles>(EnumTools.GetSelectedFlags(Roles));
            if (flags.Contains(OperatorRoles.Driver))
            {
                var proxy = new DriverDelegate(_ardSys);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.FireControl))
            {
                var proxy = new FireControlDelegate(_ardSys);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.GunLoader))
            {
                var proxy = new GunLoaderDelegate(_ardSys);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.GunRotation))
            {
                var proxy = new GunRotationDelegate(_ardSys);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.Navigator))
            {
                var proxy = new NavigatorDelegate(_ardSys);
                AddOperatorHandler(proxy);
                _operatorDelegates.Add(proxy);
            }
            if (flags.Contains(OperatorRoles.RangeFinder))
            {
                var proxy = new RangeFinderDelegate(_ardSys);
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
                var currentRB = arg.CurrentInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER);
                var prevRB = arg.PreviousInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER);
                var currentLB = arg.CurrentInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER);
                var prevLB = arg.PreviousInputState.Gamepad.IsButtonPressed(ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER);

                if (currentRB && !prevRB)
                {
                    proxy.Load();
                }
                if (currentLB && !prevLB)
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

        /// <summary>
        /// Unhook bindings and stop controller polling
        /// </summary>
        public void Dispose()
        {
            ControllerStateChanged = null;
            _operatorDelegates.DisposeAll();
            _operatorDelegates.Clear();
            _xboxManager.StopPolling();
            GC.SuppressFinalize(this);
        }
    }
}