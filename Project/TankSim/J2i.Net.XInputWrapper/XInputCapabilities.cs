using System.Runtime.InteropServices;

namespace J2i.Net.XInputWrapper
{
    /// <summary>
    /// Native struct - XInput controller capabilities
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputCapabilities
    {
        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public byte Type;

        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte SubType;

        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(2)]
        public short Flags;

        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(4)]
        public XInputGamepad Gamepad;

        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(16)]
        public XInputVibration Vibration;
    }
}
