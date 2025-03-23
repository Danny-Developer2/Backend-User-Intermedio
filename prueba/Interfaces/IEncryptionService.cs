using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prueba.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText, string secretKey);
        string Decrypt(string cipherText, string secretKey);
    }
}