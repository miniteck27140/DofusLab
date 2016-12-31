﻿using System;

namespace DofusLab.Core.IO.Types
{
    public class CustomInt64 : Binary64
    {
        public CustomInt64()
        {
        }

        public CustomInt64(uint low = 0, uint high = 0)
            : base(low, high)
        {
        }

        public uint high
        {
            get { return InternalHigh; }
            set { InternalHigh = value; }
        }

        public static CustomInt64 fromNumber(long n)
        {
            return new CustomInt64((uint)n, (uint)Math.Floor(n / 4.294967296E9));
        }

        public long toNumber()
        {
            return (long)(high * 4.294967296E9 + Low);
        }
    }
}
