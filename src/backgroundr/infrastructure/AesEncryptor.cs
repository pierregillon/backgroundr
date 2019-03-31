using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class AesEncryptor : IEncryptor
    {
        private readonly string _encryptionKey;

        public AesEncryptor(string encryptionKey)
        {
            if (encryptionKey == null) throw new ArgumentNullException(nameof(encryptionKey));
            _encryptionKey = encryptionKey;
        }

        public string Encrypt(string clearText)
        {
            try {
                var clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (var aes = Aes.Create()) {
                    var pdb = GetPdb(_encryptionKey);
                    aes.Key = pdb.GetBytes(32);
                    aes.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream()) {
                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            catch (CryptographicException ex) {
                throw new Exception("Unable to encrypted the given text.", ex);
            }
        }
        public string Decrypt(string cipherText)
        {
            try {
                cipherText = cipherText.Replace(" ", "+");
                var cipherBytes = Convert.FromBase64String(cipherText);
                using (var aes = Aes.Create()) {
                    var pdb = GetPdb(_encryptionKey);
                    aes.Key = pdb.GetBytes(32);
                    aes.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream()) {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (CryptographicException ex) {
                throw new Exception("The encrypted text cannot be decrypted.", ex);
            }
        }

        private Rfc2898DeriveBytes GetPdb(string encryptionKey)
        {
            return new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        }
    }
}