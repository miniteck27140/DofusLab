using System;

namespace DofusLab.Core.IO.Readers
{
    public unsafe partial class DofusBinaryReader : IReader, IDisposable
    {
        public T ReadValue<T>() where T : struct
            => ReaderCacheValue<T>.Read(this);

        public T ReadRef<T>() where T : class
            => ReaderCacheRef<T>.Read(this);

        public int BytesAvailable => Data.Length - Position;

        public byte[] Data { get; set; }

        public int Position { get; set; }

        public int Length => Data.Length;

        public DofusBinaryReader(byte[] buffer)
        {
            Data = buffer;
            Position = 0;
        }

        public DofusBinaryReader(byte[] buffer, int length)
        {
            var data = buffer;
            var finalData = new byte[length];

            Array.Copy(data, finalData, finalData.Length);
            Data = finalData;
            Position = 0;
        }

        static DofusBinaryReader()
        {
            ReaderCacheValue<byte>.Read = (r) => r.ReadByte();
            ReaderCacheValue<sbyte>.Read = (r) => r.ReadSByte();

            ReaderCacheValue<bool>.Read = (r) => r.ReadBoolean();

            ReaderCacheValue<short>.Read = (r) => r.ReadShort();
            ReaderCacheValue<ushort>.Read = (r) => r.ReadUShort();
            ReaderCacheValue<char>.Read = (r) => r.ReadChar();

            ReaderCacheValue<int>.Read = (r) => r.ReadInt();
            ReaderCacheValue<uint>.Read = (r) => r.ReadUInt();

            ReaderCacheValue<float>.Read = (r) => r.ReadFloat();

            ReaderCacheValue<long>.Read = (r) => r.ReadLong();
            ReaderCacheValue<ulong>.Read = (r) => r.ReadULong();
            ReaderCacheValue<double>.Read = (r) => r.ReadDouble();

            ReaderCacheRef<string>.Read = (r) => r.ReadUTF();
        }

        private static class ReaderCacheValue<T> where T : struct
        {
            public static Func<DofusBinaryReader, T> Read { get; set; }
        }

        private static class ReaderCacheRef<T> where T : class
        {
            public static Func<DofusBinaryReader, T> Read { get; set; }
        }

        public byte[] ReadBytes(int n)
        {
            var numArray = new byte[n];
            fixed (byte* numPtr1 = &Data[Position])
            fixed (byte* numPtr2 = numArray)
            {
                byte* numPtr3 = numPtr1;
                byte* numPtr4 = numPtr2;
                for (var index = 0; index < n / 4; ++index)
                {
                    *(int*)numPtr4 = *(int*)numPtr3;
                    numPtr4 += 4;
                    numPtr3 += 4;
                }
                for (var index = 0; index < n % 4; ++index)
                {
                    *numPtr4 = *numPtr3;
                    ++numPtr4;
                    ++numPtr3;
                }
            }
            Position = Position + n;
            return numArray;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Data != null)
                    Data = null;
                Position = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
            => $"Lenght: {Length} -  Position: {Position} - BytesAvailables: {BytesAvailable}";
    }
}
