using System;
using System.IO;
using System.Text;

namespace DofusLab.Core.IO.Writers
{
    public partial class DofusBinaryWriter : IWriter, IDisposable
    {
        private BinaryWriter _writer;

        public Stream BaseStream 
            => _writer.BaseStream;

        public long BytesAvailable 
            => _writer.BaseStream.Length - _writer.BaseStream.Position;

        public long Position
        {
            get
            {
                return _writer.BaseStream.Position;
            }
            set
            {
               _writer.BaseStream.Position = value;
            }
        }

        public int Length => Data.Length;

        public byte[] Data
        {
            get
            {
                var position = _writer.BaseStream.Position;
                var buffer = new byte[_writer.BaseStream.Length];
                _writer.BaseStream.Position = 0L;
                _writer.BaseStream.Read(buffer, 0, (int)_writer.BaseStream.Length);
                _writer.BaseStream.Position = position;
                return buffer;
            }
        }

        public DofusBinaryWriter()
        {
            _writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        }

        public DofusBinaryWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        public DofusBinaryWriter(byte[] buffer)
        {
            _writer = new BinaryWriter(new MemoryStream(buffer));
        }

        static DofusBinaryWriter()
        {
            MapperValue<byte>.Register((w, i) => w.WriteByte(i));
            MapperValue<sbyte>.Register((w, i) => w.WriteSByte(i));
            MapperValue<char>.Register((w, i) => w.WriteChar(i));

            MapperValue<bool>.Register((w, i) => w.WriteBoolean(i));

            MapperValue<short>.Register((w, i) => w.WriteShort(i));
            MapperValue<ushort>.Register((w, i) => w.WriteUShort(i));

            MapperValue<int>.Register((w, i) => w.WriteInt(i));
            MapperValue<uint>.Register((w, i) => w.WriteUInt(i));

            MapperValue<long>.Register((w, i) => w.WriteLong(i));
            MapperValue<ulong>.Register((w, i) => w.WriteULong(i));

            MapperRef<string>.Register((w, i) => w.WriteUTF((i)));
        }

        public void WriteValue<T>(T val) where T : struct => MapperValue<T>.Write(this, val);
        public void WriteRef<T>(T val) where T : class => MapperRef<T>.Write(this, val);

        private static class MapperValue<T> where T : struct
        {
            public static Action<DofusBinaryWriter, T> Write { get; private set; }
            public static void Register(Action<DofusBinaryWriter, T> writer) => Write = writer;
        }

        private static class MapperRef<T> where T : class
        {
            public static Action<DofusBinaryWriter, T> Write { get; private set; }
            public static void Register(Action<DofusBinaryWriter, T> writer) => Write = writer;
        }

        public void Seek(int offset, SeekOrigin seekOrigin)
        {
            _writer.BaseStream.Seek(offset, seekOrigin);
        }

        public void Clear()
        {
            _writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        }

        public void WriteBytes(byte[] buffer)
        {
            for (var index = buffer.Length - 1; index >= 0; --index)
                _writer.Write(buffer[index]);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Data != null)
                    Clear();
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
