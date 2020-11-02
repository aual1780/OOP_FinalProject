using System;
using System.Runtime.InteropServices;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace J2i.Net.XInputWrapper
{
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputState
    {
        [FieldOffset(0)]
        public int PacketNumber;

        [FieldOffset(4)]
        public XInputGamepad Gamepad;

        public void Copy(XInputState source)
        {
            PacketNumber = source.PacketNumber;
            Gamepad.Copy(source.Gamepad);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!(obj is XInputState)))
                return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                PacketNumber,
                Gamepad);
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
