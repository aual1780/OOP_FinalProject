using System;
using System.Collections.Generic;
using System.Text;

namespace J2i.Net.XInputWrapper
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XboxController
    {
        /// <summary>
        /// DPad inputs
        /// </summary>
        public class DPadState
        {
            private readonly XboxController _controller;

            internal DPadState(XboxController Controller)
            {
                _controller = Controller;
            }

            /// <summary>
            /// Dpad Up
            /// </summary>
            public bool IsUpPressed
            {
                get
                {
                    return _controller.GamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_UP);
                }
            }

            /// <summary>
            /// Dpad Down
            /// </summary>
            public bool IsDownPressed
            {
                get
                {
                    return _controller.GamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN);
                }
            }

            /// <summary>
            /// Dpad Left
            /// </summary>
            public bool IsLeftPressed
            {
                get
                {
                    return _controller.GamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT);
                }
            }

            /// <summary>
            /// Dpad Right
            /// </summary>
            public bool IsRightPressed
            {
                get
                {
                    return _controller.GamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT);
                }
            }
        }
    }
}
