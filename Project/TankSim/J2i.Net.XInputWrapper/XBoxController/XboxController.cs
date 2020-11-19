using System;
using System.Threading;

namespace J2i.Net.XInputWrapper
{
    /// <summary>
    /// Controller state tracking
    /// </summary>
    public partial class XboxController
    {
        readonly int _playerIndex;
        bool _stopMotorTimerActive;
        DateTime _stopMotorTime;
        XInputBatteryInformation _batteryInformationGamepad;
        XInputBatteryInformation _batterInformationHeadset;
        private XInputState _gamepadStatePrev = new();
        private XInputState _gamepadStateCurrent = new();

        //XInputCapabilities _capabilities;

        internal XInputState GamepadStateCurrent
        {
            get => _gamepadStateCurrent;
        }


        /// <summary>
        /// Determine if this controller object has an associated physical controller
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gamepad battery info
        /// </summary>
        public XInputBatteryInformation BatteryInformationGamepad
        {
            get { return _batteryInformationGamepad; }
            internal set { _batteryInformationGamepad = value; }
        }

        /// <summary>
        /// Headset batter info
        /// </summary>
        public XInputBatteryInformation BatteryInformationHeadset
        {
            get { return _batterInformationHeadset; }
            internal set { _batterInformationHeadset = value; }
        }

        /// <summary>
        /// Controller state changed event
        /// </summary>
        public event EventHandler<XboxControllerStateChangedEventArgs> StateChanged = null;



        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="playerIndex"></param>
        internal XboxController(int playerIndex)
        {
            _playerIndex = playerIndex;
            DPad = new DPadState(this);
            _gamepadStatePrev.Copy(_gamepadStateCurrent);
        }

        /// <summary>
        /// Request battery info from controller.
        /// Call this before checking battery level
        /// </summary>
        public void UpdateBatteryState()
        {
            var gamepad = new XInputBatteryInformation();
            var headset = new XInputBatteryInformation();

            _ = XInput.XInputGetBatteryInformation(_playerIndex, (byte)BatteryDeviceType.BATTERY_DEVTYPE_GAMEPAD, ref gamepad);
            _ = XInput.XInputGetBatteryInformation(_playerIndex, (byte)BatteryDeviceType.BATTERY_DEVTYPE_HEADSET, ref headset);

            BatteryInformationGamepad = gamepad;
            BatteryInformationHeadset = headset;
        }

        /// <summary>
        /// Trigger controller state changed event
        /// </summary>
        protected void OnStateChanged()
        {
            var arg = new XboxControllerStateChangedEventArgs()
            {
                CurrentInputState = _gamepadStateCurrent,
                PreviousInputState = _gamepadStatePrev
            };
            StateChanged?.Invoke(this, arg);
        }

        /// <summary>
        /// Get controller capabilities
        /// </summary>
        /// <returns></returns>
        public XInputCapabilities GetCapabilities()
        {
            XInputCapabilities capabilities = new XInputCapabilities();
            _ = XInput.XInputGetCapabilities(_playerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, ref capabilities);
            return capabilities;
        }


        #region Digital Button States

        /// <summary>
        /// Dpad input state
        /// </summary>
        public DPadState DPad { get; }

        /// <summary>
        /// A
        /// </summary>
        public bool IsAPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_A); }
        }

        /// <summary>
        /// B
        /// </summary>
        public bool IsBPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_B); }
        }

        /// <summary>
        /// X
        /// </summary>
        public bool IsXPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_X); }
        }

        /// <summary>
        /// Y
        /// </summary>
        public bool IsYPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_Y); }
        }

        /// <summary>
        /// Back
        /// </summary>
        public bool IsBackPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_BACK); }
        }

        /// <summary>
        /// Start
        /// </summary>
        public bool IsStartPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_START); }
        }

        /// <summary>
        /// Left bumper
        /// </summary>
        public bool IsLeftShoulderPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER); }
        }

        /// <summary>
        /// Right bumper
        /// </summary>
        public bool IsRightShoulderPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER); }
        }

        /// <summary>
        /// Left stick
        /// </summary>
        public bool IsLeftStickPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB); }
        }

        /// <summary>
        /// Right stick
        /// </summary>
        public bool IsRightStickPressed
        {
            get { return _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB); }
        }
        #endregion

        #region Analogue Input States
        /// <summary>
        /// Left trigger state
        /// </summary>
        public int LeftTrigger => _gamepadStateCurrent.Gamepad.bLeftTrigger;

        /// <summary>
        /// Right trigger state
        /// </summary>
        public int RightTrigger => _gamepadStateCurrent.Gamepad.bRightTrigger;

        /// <summary>
        /// Left thumbstick X/Y orientation
        /// </summary>
        public Point LeftThumbStick
        {
            get
            {
                Point p = new Point()
                {
                    X = _gamepadStateCurrent.Gamepad.sThumbLX,
                    Y = _gamepadStateCurrent.Gamepad.sThumbLY
                };
                return p;
            }
        }

        /// <summary>
        /// Right thumbstick X/Y orientation
        /// </summary>
        public Point RightThumbStick
        {
            get
            {
                Point p = new Point()
                {
                    X = _gamepadStateCurrent.Gamepad.sThumbRX,
                    Y = _gamepadStateCurrent.Gamepad.sThumbRY
                };
                return p;
            }
        }

        #endregion


        #region Polling


        #region Motor Functions
        /// <summary>
        /// Vibrate motors for short time
        /// </summary>
        /// <param name="leftMotor"></param>
        /// <param name="rightMotor"></param>
        public void Vibrate(double leftMotor, double rightMotor)
        {
            Vibrate(leftMotor, rightMotor, TimeSpan.MinValue);
        }

        /// <summary>
        /// Vibrate motors
        /// </summary>
        /// <param name="leftMotor"></param>
        /// <param name="rightMotor"></param>
        /// <param name="length"></param>
        public void Vibrate(double leftMotor, double rightMotor, TimeSpan length)
        {
            leftMotor = Math.Max(0d, Math.Min(1d, leftMotor));
            rightMotor = Math.Max(0d, Math.Min(1d, rightMotor));

            XInputVibration vibration = new XInputVibration() { LeftMotorSpeed = (ushort)(65535d * leftMotor), RightMotorSpeed = (ushort)(65535d * rightMotor) };
            Vibrate(vibration, length);
        }

        /// <summary>
        /// Vibrate motors
        /// </summary>
        /// <param name="strength"></param>
        public void Vibrate(XInputVibration strength)
        {
            _stopMotorTimerActive = false;
            _ = XInput.XInputSetState(_playerIndex, ref strength);
        }

        /// <summary>
        /// Vibrate motors
        /// </summary>
        /// <param name="strength"></param>
        /// <param name="length"></param>
        public void Vibrate(XInputVibration strength, TimeSpan length)
        {
            _ = XInput.XInputSetState(_playerIndex, ref strength);
            if (length != TimeSpan.MinValue)
            {
                _stopMotorTime = DateTime.Now.Add(length);
                _stopMotorTimerActive = true;
            }
        }
        #endregion


        /// <summary>
        /// Manually poll controller for current state
        /// </summary>
        public void UpdateState()
        {
            int result = XInput.XInputGetState(_playerIndex, ref _gamepadStateCurrent);
            IsConnected = (result == 0);

            UpdateBatteryState();
            if (_gamepadStateCurrent.PacketNumber != _gamepadStatePrev.PacketNumber)
            {
                OnStateChanged();
            }
            _gamepadStatePrev.Copy(_gamepadStateCurrent);

            if (_stopMotorTimerActive && (DateTime.Now >= _stopMotorTime))
            {
                XInputVibration stopStrength = new XInputVibration() { LeftMotorSpeed = 0, RightMotorSpeed = 0 };
                _ = XInput.XInputSetState(_playerIndex, ref stopStrength);
            }
        }
        #endregion


        /// <summary>
        /// Return player index
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _playerIndex.ToString();
        }

    }
}
