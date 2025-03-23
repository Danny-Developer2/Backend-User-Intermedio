using System.Security.Cryptography;
using System.Text;
using prueba.Interfaces;

namespace prueba.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string Encrypt(string plainText, string secretKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = DeriveKey(secretKey, aesAlg.KeySize / 8);
                aesAlg.IV = DeriveKey(secretKey, aesAlg.BlockSize / 8);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText, string secretKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = DeriveKey(secretKey, aesAlg.KeySize / 8);
                aesAlg.IV = DeriveKey(secretKey, aesAlg.BlockSize / 8);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private static byte[] DeriveKey(string secretKey, int keySize)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey)).AsSpan(0, keySize).ToArray();
            }
        }
    }
}