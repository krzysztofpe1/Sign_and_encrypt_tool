using System.Security.Cryptography;
using System.Text;

namespace SignAndEncyptTool.KeysManagement;

internal class AESManager
{
    public static string DecryptAES(string encryptedText, string passphrase)
    {
        byte[] cipherBytes = Convert.FromBase64String(encryptedText);
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GenerateKey(passphrase, aesAlg.KeySize);
            aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // Initialization vector is not used in this example, but it should be if you're using AES for real encryption

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    public static string EncryptAES(string plaintext, string passphrase)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GenerateKey(passphrase, aesAlg.KeySize);
            aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // Initialization vector is not used in this example, but it should be if you're using AES for real encryption

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plaintext);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    private static byte[] GenerateKey(string passphrase, int keySize)
    {
        byte[] key = new byte[keySize / 8];
        byte[] salt = Encoding.UTF8.GetBytes("YourSaltValue");

        using (var deriveBytes = new Rfc2898DeriveBytes(passphrase, salt))
        {
            key = deriveBytes.GetBytes(keySize / 8);
        }
        return key;
    }
}
