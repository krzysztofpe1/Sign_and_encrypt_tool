using System.Security.Cryptography;
using System.Text;

namespace SignAndEncyptTool.KeysManagement;

internal class AESManager
{
    public static string DecryptAES(string encryptedText, string passphrase)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        using Aes aes = Aes.Create();
        aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(passphrase));
        aes.IV = new byte[16]; // Assuming the IV is stored along with the encrypted data

        using MemoryStream memoryStream = new MemoryStream(encryptedBytes);
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
    public static string EncryptAES(string plaintext, string passphrase)
    {
        using Aes aes = Aes.Create();
        aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(passphrase));
        var iv = new byte[16];
        new Random().NextBytes(iv);
        aes.IV = iv;

        using MemoryStream memoryStream = new MemoryStream();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using StreamWriter streamWriter = new StreamWriter(cryptoStream);

        streamWriter.Write(plaintext);
        streamWriter.Flush();
        cryptoStream.FlushFinalBlock();

        return Convert.ToBase64String(memoryStream.ToArray());
    }
}
