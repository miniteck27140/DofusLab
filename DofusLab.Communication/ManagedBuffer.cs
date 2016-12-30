using System;
using System.Runtime.CompilerServices;

namespace DofusLab.Communication
{
    public class ManagedBuffer : IDisposable
    {
        private readonly BufferPool _mPool;

        public byte[] Buffer { get; private set; }
        public int RealSize { get; private set; }

        public void Resize(int newSize) //check if reduce or enlarge!
        {
            if (Buffer.Length == GetNearestPower(newSize))
                return;

            var buffer = _mPool.Take(newSize);

            Array.Copy(Buffer, 0, buffer, 0, newSize > RealSize ? RealSize : buffer.Length);

            //release the old buffer
            FreeBuffer();

            Buffer = buffer;
            RealSize = newSize;
        }

        public ManagedBuffer(BufferPool pool, int bufferSize)
        {
            _mPool = pool;

            Buffer = pool.Take(bufferSize);
            RealSize = bufferSize;
        }

        private static int GetNearestPower(int num)
        {
            var n = num > 0 ? --num : 0;

            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            n++;
            return n;
        }

        // Free the buffer and back to pool.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FreeBuffer()
        {
            _mPool.Return(Buffer);
            Buffer = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Buffer != null)
                    FreeBuffer();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
            => $"RealSize: {RealSize} - Buffer.Length : {Buffer.Length} - Display first byte: {Buffer[0]}";
    }
}
