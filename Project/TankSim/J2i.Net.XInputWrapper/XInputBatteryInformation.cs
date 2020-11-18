using System.Runtime.InteropServices;

namespace J2i.Net.XInputWrapper
{
    /// <summary>
    /// Native XInput struct - controller battery settings
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputBatteryInformation
    {
        /// <summary>
        /// Controller battery type
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public byte BatteryType;

        /// <summary>
        /// Controller battery level
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte BatteryLevel;

        /// <summary>
        /// Print battery type and level
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", (BatteryTypes)BatteryType, (BatteryLevel)BatteryLevel);
        }
    }
}
