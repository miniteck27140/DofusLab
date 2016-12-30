using System.Buffers;
using DofusLab.Communication.Interfaces;

namespace DofusLab.Communication
{
    public class BufferPool : IBufferPool
    {
        public int MaxBufferLength { get; }

        public void Return(byte[] buffer) => Pool.Return(buffer);
        public byte[] Take(int bufferSize) => Pool.Rent(bufferSize);

        public ManagedBuffer GetManagedBuffer(int bufferSize)
            => new ManagedBuffer(this, bufferSize);

        public ArrayPool<byte> Pool { get; }

        public BufferPool(int maxBufferLength, int numberBuckets)
        {
            Pool = ArrayPool<byte>.Create(maxBufferLength, numberBuckets);
            MaxBufferLength = maxBufferLength;
        }
    }
}
