using System.Security.Cryptography;

namespace Listings_Portal.Lib.Tools.Managers
{
    /// <summary>
    /// Security manager that provides an internal cryption key.
    /// </summary>
    public static class SecurityManager
    {
        private static readonly byte[] cryptionKey = [
            0x32, 0x64, 0x35, 0x32, 0x37, 0x35, 0x38, 0x66,
            0x35, 0x32, 0x39, 0x65, 0x62, 0x61, 0x62, 0x33,
            0x33, 0x66, 0x34, 0x36, 0x33, 0x35, 0x35, 0x39,
            0x35, 0x63, 0x66, 0x30, 0x66, 0x32, 0x62, 0x38
        ];

        private static readonly byte[] iv = [
            0x36, 0x37, 0x33, 0x39, 0x66, 0x37, 0x34, 0x36,
            0x62, 0x62, 0x66, 0x33, 0x32, 0x38, 0x38, 0x32
        ];

        /// <summary> Gets AES algorithm handler for internal cryption properties. </summary>
        /// <returns> AES algorithm handler. </returns>
        private static Aes GetAes()
        {
            Aes aes = Aes.Create();
            aes.Key = cryptionKey;
            aes.IV = iv;
            return aes;
        }

        /// <summary>
        /// Encrypts the specified plain text using this manager's internal cryption key.
        /// </summary>
        /// <param name="plainText"> Plain text to encrypt. </param>
        /// <returns> Encrypted bytes. </returns>
        public static byte[] Encrypt(string plainText)
        {
            using Aes aesAlg = GetAes();
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                swEncrypt.Write(plainText);
            return msEncrypt.ToArray();
        }

        /// <summary>
        /// Decrypts the specified encrypted bytes using this manager's internal cryption key.
        /// The encrypted bytes must have been encrypted with this manager's internal cryption key.
        /// </summary>
        /// <param name="encryptedBytes"> Encrypted bytes to decrypt. </param>
        /// <returns> Decrypted plain text. </returns>
        public static string Decrypt(byte[] encryptedBytes)
        {
            using Aes aesAlg = GetAes();
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(encryptedBytes);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
