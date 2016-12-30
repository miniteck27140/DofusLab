using System.Linq;
using System.Security.Cryptography;
using static System.Console;

namespace DofusLab.Core.Cryptography
{
    public class Cryptography
    {
        #region AES
        public static byte[] EncryptAES(byte[] data, byte[] key)
        {
            var iv = key.Take(16).ToArray();
            try
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
                {
                    var crypto = rijndaelManaged.CreateEncryptor();

                    return crypto.TransformFinalBlock(data, 0, data.Length);
                }
            }
            catch (CryptographicException e)
            {
                WriteLine($"A Cryptographic error occurred: {e.Message}");
                return null;
            }
        }

        public static byte[] DecryptAES(byte[] data, byte[] key)
        {
            var iv = key;
            try
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
                {
                    var crypto = rijndaelManaged.CreateDecryptor(key, iv);

                    return crypto.TransformFinalBlock(data, 0, data.Length);
                }
            }
            catch (CryptographicException e)
            {
                WriteLine($"A Cryptographic error occurred: {e.Message}");
                return null;
            }
        }
        #endregion
    }
}
