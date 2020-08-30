using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Pixel.Encryption
{
    public class AES
    {
        private const string NULL_IV = "0000000000000000";

        public static string Encrypt(string input, string key, string iv = NULL_IV)
        {
            key = MD5Hasher.Hash(key);

            return Convert.ToBase64String(EncryptStringToBytes(
                input,
                RawBytesFromString(key),
                RawBytesFromString(iv)
            ));
        }

        public static string Decrypt(string inputBase64, string key, string iv = NULL_IV)
        {
            key = MD5Hasher.Hash(key);

            return DecryptStringFromBytes(
                Convert.FromBase64String(inputBase64),
                RawBytesFromString(key),
                RawBytesFromString(iv)
            );
        }

        private static byte[] RawBytesFromString(string input)
        {
            return input.Select(x => (byte) ((ulong) x & 0xFF)).ToArray();
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            byte[] encrypted;

            using (var cipher = new RijndaelManaged())
            {
                cipher.Key = key;
                cipher.IV = iv;
                //cipher.Mode = CipherMode.CBC;
                //cipher.Padding = PaddingMode.PKCS7;
                
                var encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);
                
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            
            string plaintext;
            
            using (var cipher = new RijndaelManaged())
            {
                cipher.Key = key;
                cipher.IV = iv;
                //cipher.Mode = CipherMode.CBC;
                //cipher.Padding = PaddingMode.PKCS7;
                
                var decryptor = cipher.CreateDecryptor(cipher.Key, cipher.IV);
                
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}