using System;
using System.Text;

namespace DofusLab.Core.IO.Readers
{
    public unsafe partial class DofusBinaryReader
    {
        #region Private Func
        private byte ReadByte()
        {
            var buffer = Data;
            var position = Position;
            Position = position + 1;
            fixed (byte* numPtr = &buffer[position])
                return *numPtr;
        }

        private sbyte ReadSByte()
        {
            var buffer = Data;
            var position = Position;
            Position = position + 1;
            fixed (byte* numPtr = &buffer[position])
                return (sbyte)*numPtr;
        }

        private bool ReadBoolean() => ReadByte() > 0U;

        private short ReadShort()
        {
            var position = Position;
            Position = Position + 2;
            fixed (byte* numPtr = &Data[position])
                return (short)((*numPtr) << 8 | numPtr[1]);
        }

        private ushort ReadUShort() => (ushort)ReadShort();

        private char ReadChar() => (char)ReadShort();

        private int ReadInt()
        {
            var position = Position;
            Position = Position + 4;
            fixed (byte* numPtr = &Data[position])
                return (*numPtr) << 24 | numPtr[1] << 16 | numPtr[2] << 8 | numPtr[3];
        }

        private uint ReadUInt() => (uint)ReadInt();

        private long ReadLong()
        {
            var position = Position;
            Position = Position + 8;
            fixed (byte* numPtr = &Data[position])
            {
                var num = (*numPtr) << 24 | numPtr[1] << 16 | numPtr[2] << 8 | numPtr[3];
                return ((uint)(numPtr[4] << 24 | numPtr[5] << 16 | numPtr[6] << 8) | numPtr[7]) | (long)num << 32;
            }
        }


        private ulong ReadULong() => (ulong)ReadLong();

        private float ReadFloat()
        {
            var i = ReadInt();
            return *(float*)&i;
        }

        private double ReadDouble()
        {
            var l = ReadLong();
            return *(double*) &l;
        }

        private string ReadUTF() => Encoding.UTF8.GetString(ReadBytes(ReadUShort()));

        private string ReadUTFBytes(ushort len) => Encoding.UTF8.GetString(ReadBytes(len));
        #endregion

        #region Custom func
        public int ReadVarInt()
        {
            var num1 = 0;
            var num2 = 0;
            while (num2 < 32)
            {
                var num3 = (int)ReadByte();
                var flag = (num3 & 128) == 128;
                if (num2 > 0)
                    num1 += (num3 & 128) << num2;
                else
                    num1 += num3 & sbyte.MaxValue;
                num2 += 7;
                if (!flag)
                    return num1;
            }
            throw new Exception("Too much data");
        }

        public uint ReadVarUhInt() => (uint)ReadVarInt();

        public int ReadVarShort()
        {
            var num1 = 0;
            var num2 = 0;
            while (num2 < 16)
            {
                var num3 = (int)this.ReadByte();
                var flag = (num3 & 128) == 128;
                if (num2 > 0)
                    num1 += (num3 & 128) << num2;
                else
                    num1 += num3 & sbyte.MaxValue;
                num2 += 7;
                if (flag) continue;
                if (num1 > short.MaxValue)
                    num1 -= 65536;
                return num1;
            }
            throw new Exception("Too much data");
        }

        public uint ReadVarUhShort() => (uint)ReadVarShort();

        public double ReadVarLong() => ReadInt();

        public double ReadVarUhLong() => ReadUInt();
        #endregion
    }
}
