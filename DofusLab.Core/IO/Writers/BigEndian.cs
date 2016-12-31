using System;
using System.Text;
using DofusLab.Core.IO.Readers;
using DofusLab.Core.IO.Types;

namespace DofusLab.Core.IO.Writers
{
    public partial class DofusBinaryWriter
    {
        #region PrivateFunc
        private void WriteByte(byte @byte) => _writer.Write(@byte);
        private void WriteSByte(sbyte @byte) => _writer.Write(@byte);

        private void WriteBoolean(bool @bool)
        {
            if (@bool)
                _writer.Write((byte)1);
            else
                _writer.Write((byte)0);
        }

        private void WriteChar(char @char) => WriteByteArray(BitConverter.GetBytes(@char));

        private void WriteShort(short @short) => WriteByteArray(BitConverter.GetBytes(@short));
        private void WriteUShort(ushort @ushort) => WriteByteArray(BitConverter.GetBytes(@ushort));

        private void WriteInt(int @int) => WriteByteArray(BitConverter.GetBytes(@int));
        private void WriteUInt(uint @uint) => WriteByteArray(BitConverter.GetBytes(@uint));

        private void WriteLong(long @long) => WriteByteArray(BitConverter.GetBytes(@long));
        private void WriteULong(ulong @ulong) => WriteByteArray(BitConverter.GetBytes(@ulong));

        private void WriteFloat(float @float) => _writer.Write(@float);
        private void WriteSingle(float single) => WriteByteArray(BitConverter.GetBytes(single));
        private void WriteDouble(double @double) => WriteByteArray(BitConverter.GetBytes(@double));

        private void WriteUTF(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var length = (ushort)bytes.Length;
            WriteUShort(length);
            for (var index = 0; index < (int)length; ++index)
                _writer.Write(bytes[index]);
        }

        private void WriteUTFBytes(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var length = bytes.Length;
            for (var index = 0; index < length; ++index)
                _writer.Write(bytes[index]);
        }

        public void WriteByteArray(byte[] data)
        {
            _writer.Write(data);
        }

        public void WriteBytes(byte[] data, uint offset, uint length)
        {
            var buffer = new byte[(int)length];
            Array.Copy(data, offset, buffer, 0L, length);
            _writer.Write(buffer);
        }

        #endregion

        #region CustomVar

        public void WriteVarInt(int @int)
        {
            var bigEndianWriter1 = new DofusBinaryWriter();
            if (@int >= 0 && @int <= sbyte.MaxValue)
            {
                bigEndianWriter1.WriteByte((byte)@int);
                WriteBytes(bigEndianWriter1.Data);
            }
            else
            {
                var num1 = @int;
                var bigEndianWriter2 = new DofusBinaryWriter();
                while ((uint)num1 > 0U)
                {
                    bigEndianWriter2.WriteByte((byte)(num1 & sbyte.MaxValue));
                    var bigEndianReader = new DofusBinaryReader(bigEndianWriter2.Data);
                    var num2 = (int)bigEndianReader.ReadValue<byte>();
                    bigEndianWriter2 = new DofusBinaryWriter(bigEndianReader.Data);
                    num1 = (int)((uint)num1 >> 7);
                    if (num1 > 0)
                        num2 |= 128;
                    bigEndianWriter1.WriteByte((byte)num2);
                }
                WriteBytes(bigEndianWriter1.Data);
            }
        }

        public void WriteVarShort(int @int)
        {
            if (@int > short.MaxValue || @int < short.MinValue)
                throw new Exception("Forbidden value");
            var bigEndianWriter1 = new DofusBinaryWriter();
            if (@int >= 0 && @int <= sbyte.MaxValue)
            {
                bigEndianWriter1.WriteByte((byte)@int);
                WriteBytes(bigEndianWriter1.Data);
            }
            else
            {
                var num1 = @int & ushort.MaxValue;
                var bigEndianWriter2 = new DofusBinaryWriter();
                while ((uint)num1 > 0U)
                {
                    bigEndianWriter2.WriteByte((byte)(num1 & sbyte.MaxValue));
                    var bigEndianReader = new DofusBinaryReader(bigEndianWriter2.Data);
                    var num2 = (int)bigEndianReader.ReadValue<byte>();
                    bigEndianWriter2 = new DofusBinaryWriter(bigEndianReader.Data);
                    num1 = (int)((uint)num1 >> 7);
                    if (num1 > 0)
                        num2 |= 128;
                    bigEndianWriter1.WriteByte((byte)num2);
                }
                WriteBytes(bigEndianWriter1.Data);
            }
        }

        public void WriteVarLong(double @double)
        {
            var finalInt64 = CustomInt64.fromNumber((long)@double);
            if (finalInt64.high == 0)
            {
                WriteInt32(finalInt64.Low);
            }
            else
            {
                for (uint index = 0; index < 4U; ++index)
                {
                    WriteByte((byte)((int)finalInt64.Low & sbyte.MaxValue | 128));
                    finalInt64.Low = (uint)((long)finalInt64.Low >> 7);
                }
                if ((finalInt64.high & 2147483640) == 0)
                {
                    WriteByte((byte)((finalInt64.high << 4) | finalInt64.Low));
                }
                else
                {
                    WriteByte((byte)(((int)finalInt64.high << 4 | (int)finalInt64.Low) & sbyte.MaxValue | 128));
                    WriteInt32(finalInt64.high >> 3);
                }
            }
        }

        private void WriteInt32(uint param1)
        {
            while (param1 >= 128U)
            {
                WriteByte((byte)((int)param1 & sbyte.MaxValue | 128));
                param1 >>= 7;
            }
            WriteByte((byte)param1);
        }

        #endregion
    }
}
