using System.Security.Cryptography.X509Certificates;

namespace DofusLab.Core.IO
{
    public interface IReader
    {
        byte[] Data { get; set; }
        int Position { get; set; }
        int BytesAvailable { get; }
        int Length { get; }

        byte[] ReadBytes(int n);

        T ReadValue<T>() where T : struct;
        T ReadRef<T>() where T : class;

        int ReadVarInt();
        uint ReadVarUhInt();
        int ReadVarShort();
        uint ReadVarUhShort();
        double ReadVarLong();
        double ReadVarUhLong();
    }
}
