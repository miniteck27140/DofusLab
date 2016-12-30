namespace DofusLab.Communication.Interfaces
{
    public interface IBufferPool
    {
        byte[] Take(int bufferSize);
        void Return(byte[] buffer);
        int MaxBufferLength { get; }

        ManagedBuffer GetManagedBuffer(int bufferSize);
    }
}
