using System;

namespace DofusLab.Core.IO
{
    public class CustomConst
    {
        public const int INT_SIZE = 32;

        public const int SHORT_SIZE = 16;

        public const int SHORT_MIN_VALUE = -32768;

        public const int SHORT_MAX_VALUE = 32767;

        public const int UNSIGNED_SHORT_MAX_VALUE = 65536;

        public const int CHUNCK_BIT_SIZE = 7;

        public const int MASK_10000000 = 128;

        public const int MASK_01111111 = 127;

        public static int MAX_ENCODING_LENGTH = (int)Math.Ceiling((double)INT_SIZE / CHUNCK_BIT_SIZE);

        public const int MASK_1 = 128;

        public const int MASK_0 = 127;

    }
}
