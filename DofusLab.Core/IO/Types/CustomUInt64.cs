using System;

namespace DofusLab.Core.IO.Types
{
    public class CustomUInt64 : Binary64
    {
        public CustomUInt64()
        {
        }

        public CustomUInt64(uint low = 0, uint high = 0)
            : base(low, high)
        {
        }

        public uint high
        {
            get { return InternalHigh; }
            set { InternalHigh = value; }
        }

        public static CustomUInt64 fromNumber(ulong n)
        {
            return new CustomUInt64((uint)n, (uint)Math.Floor(n / 4.294967296E9));
        }

        public ulong toNumber()
        {
            return (ulong)(high * 4.294967296E9 + Low);
        }
    }
}
