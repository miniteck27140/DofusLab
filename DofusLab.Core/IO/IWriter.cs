using System.IO;

namespace DofusLab.Core.IO
{
    public interface IWriter
    {
        byte[] Data { get; }
        long Position { get; set; }
        long BytesAvailable { get; }
        int Length { get; }

        void WriteBytes(byte[] buffer);

        void WriteValue<T>(T val) where T : struct;
        void WriteRef<T>(T val) where T : class;

        void Seek(int offset, SeekOrigin seekOrigin);
        void Clear();
    }
}
