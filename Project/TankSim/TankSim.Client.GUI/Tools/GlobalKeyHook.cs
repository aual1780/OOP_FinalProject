using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TankSim.Client.GUI.Tools
{
    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    /// <remarks>
    /// KeyHook doc: https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644984(v=vs.85)
    /// Keyboard docs: https://docs.microsoft.com/en-us/windows/win32/inputdev/about-keyboard-input?redirectedfrom=MSDN
    /// lparam struct: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-kbdllhookstruct?redirectedfrom=MSDN
    /// 
    /// slightly improved version: https://gist.github.com/Lunchbox4K/291f9c8a2501170221d11d29d1355ee1
    /// </remarks>
    public class GlobalKeyHook : IDisposable
    {
        #region Constant, Structure and Delegate Definitions
        /// <summary>
        /// defines the callback type for the hook
        /// </summary>
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        /// <summary>
        /// Track keyboard keystate (down/up).  Used to determine if a key is being held or pressed repeatedly
        /// </summary>
        public bool[] KeyPressStateArr = new bool[256];

        /// <summary>
        /// Keyboard hook data
        /// </summary>
        /// <remarks>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-kbdllhookstruct?redirectedfrom=MSDN
        /// </remarks>
        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

#pragma warning disable IDE1006 // Naming Styles
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
#pragma warning restore IDE1006 // Naming Styles
        #endregion

        #region Instance Variables
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        IntPtr _hhook = IntPtr.Zero;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public event RawKeyEventHandler KeyDown = delegate { };
        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public event RawKeyEventHandler KeyUp = delegate { };
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalKeyHook"/> class and installs the keyboard hook.
        /// </summary>
        public GlobalKeyHook()
        {
            Hook();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="GlobalKeyHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
        /// </summary>
        ~GlobalKeyHook()
        {
            _ = Unhook();
        }

        /// <summary>
        /// Unhook keybinds
        /// </summary>
        public void Dispose()
        {
            KeyDown = delegate { };
            KeyUp = delegate { };
            _ = Unhook();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Installs the global hook
        /// </summary>
        private void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            _hhook = SetWindowsHookEx(WH_KEYBOARD_LL, HookProc, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        private bool Unhook()
        {
            return UnhookWindowsHookEx(_hhook);
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        public int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                bool isSysKey = (wParam == WM_SYSKEYDOWN || wParam == WM_SYSKEYUP);

                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                {
                    bool isHeld = KeyPressStateArr[lParam.vkCode];
                    var keyArg = new RawKeyEventArgs(lParam.vkCode, isSysKey, isHeld);
                    KeyPressStateArr[lParam.vkCode] = true;
                    KeyDown?.Invoke(this, keyArg);
                }
                else if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
                {
                    var keyArg = new RawKeyEventArgs(lParam.vkCode, isSysKey, false);
                    KeyPressStateArr[lParam.vkCode] = false;
                    KeyUp?.Invoke(this, keyArg);
                }
            }
            return CallNextHookEx(_hhook, code, wParam, ref lParam);
        }

        #endregion

        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion
    }


    /// <summary>
    /// Keyboard hook event arg
    /// </summary>
    public class RawKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Raw virtual keycode
        /// </summary>
        public int VKCode { get; }
        /// <summary>
        /// WPF key
        /// </summary>
        public Key Key { get; }
        /// <summary>
        /// Sys key flag
        /// </summary>
        public bool IsSysKey { get; }

        /// <summary>
        /// Determine if this key is a repeat press (indicates key held)
        /// </summary>
        public bool IsRepeat { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VKCode"></param>
        /// <param name="IsSysKey"></param>
        /// <param name="IsRepeat"></param>
        public RawKeyEventArgs(int VKCode, bool IsSysKey, bool IsRepeat)
        {
            this.VKCode = VKCode;
            this.IsSysKey = IsSysKey;
            this.Key = KeyInterop.KeyFromVirtualKey(VKCode);
            this.IsRepeat = IsRepeat;
        }
    }

    /// <summary>
    /// Event handler for raw keyboard event hooks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RawKeyEventHandler(object sender, RawKeyEventArgs e);
}