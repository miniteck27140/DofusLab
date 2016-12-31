using System;
using DofusLab.Core.IO;

namespace DofusLab.Communication
{
    public class MetadataBuilder
    {
        private byte[] m_data;

        public bool IsValid => Header.HasValue && Length.HasValue &&
                               Length == Data.Length;

        public int? Header { get; private set; }

        public int? MessageId => Header >> 2;

        public int? LengthBytesCount => Header & 0x3;

        public int? Length { get; private set; }

        public byte[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public bool Build(IReader reader)
        {
            try
            {
                if (reader.BytesAvailable <= 0)
                    return false;

                if (IsValid)
                    return true;

                if (!Header.HasValue && reader.BytesAvailable < 2)
                    return false;

                if (reader.BytesAvailable >= 2 && !Header.HasValue)
                    Header = reader.ReadValue<short>();

                if (LengthBytesCount.HasValue &&
                    reader.BytesAvailable >= LengthBytesCount && !Length.HasValue)
                {
                    if (LengthBytesCount < 0 || LengthBytesCount > 3)
                        throw new Exception(
                            "Malformated Message Header, invalid bytes number to read message length (inferior to 0 or superior to 3)");

                    Length = 0;

                    // 3..0 or 2..0 or 1..0
                    for (var i = LengthBytesCount.Value - 1; i >= 0; i--)
                    {
                        Length |= reader.ReadValue<byte>() << (i * 8);
                    }
                }

                if (Data == null && Length.HasValue)
                {
                    if (Length == 0)
                        Data = new byte[0];

                    // enough bytes in the buffer to build a complete message
                    if (reader.BytesAvailable >= Length)
                    {
                        Data = reader.ReadBytes(Length.Value);
                        return true;
                    }
                    // not enough bytes, so we read what we can
                    if (Length > reader.BytesAvailable)
                    {
                        Data = reader.ReadBytes(reader.BytesAvailable);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return IsValid;
        }
    }
}
