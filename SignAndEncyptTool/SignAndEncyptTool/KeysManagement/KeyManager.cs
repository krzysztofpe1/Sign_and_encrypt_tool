﻿using SignAndEncyptTool.Utils;
using System.Security.Cryptography;

namespace SignAndEncyptTool.KeysManagement;

public class KeyManager
{

    #region Private default file names

    private const string DEFAULT_PRIVATE_KEY_NAME = "privateKey";
    private const string DEFAULT_PUBLIC_KEY_NAME = "pubKey.pub";
    private const Int32 DEFAULT_KEY_SIZE = 2048;
    private const bool FOAEP = false;

    #endregion

    #region Private vars

    private bool _verifiedPrivateKey = false;
    private bool _verifiedPublicKey = false;
    private RSACryptoServiceProvider _rsa = new();

    #endregion

    #region Public Props

    private string _privateKeyPath = string.Empty;
    public string PrivateKeyPath
    {
        get { return _privateKeyPath; }
        set
        {
            _privateKeyPath = value;
            _verifiedPrivateKey = false;
        }
    }

    private string _publicKeyPath = string.Empty;
    public string PublicKeyPath
    {
        get { return _publicKeyPath; }
        set
        {
            _publicKeyPath = value;
            _verifiedPublicKey = false;
        }
    }

    public bool IsVerified
    {
        get
        {
            if (PrivateKeyPath.IsNullOrEmpty() && PublicKeyPath.IsNullOrEmpty())
                return false;

            if (!_verifiedPrivateKey && !_verifiedPublicKey)
                return false;

            if (!PrivateKeyPath.IsNullOrEmpty())
            {
                if (!_verifiedPrivateKey)
                    return false;
            }

            if (!PublicKeyPath.IsNullOrEmpty())
            {
                if (!_verifiedPublicKey)
                    return false;
            }

            return true;
        }
    }

    #endregion

    public KeyManager() { }

    #region Public Methods

    #region Verify

    public bool Verify(string? passphrase = null)
    {
        if (IsVerified)
            return true;

        if (PublicKeyPath == string.Empty && PrivateKeyPath == string.Empty)
            return false;

        if (PublicKeyPath != string.Empty)
        {
            if (!VerifyPublicKey())
                return false;
        }

        if (PrivateKeyPath != string.Empty)
        {
            if (!VerifyPrivateKey(passphrase))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="passphrase"></param>
    /// <returns></returns>
    /// <exception cref="SAEException"></exception>
    public bool VerifyPrivateKey(string? passphrase = null)
    {
        _verifiedPrivateKey = false;
        try
        {
            if (!File.Exists(PrivateKeyPath))
            {
                throw new SAEException("Private key file doesn't exist.");
            }

            // Read the private key from the file
            string privateKeyBase64 = File.ReadAllText(PrivateKeyPath);

            // Decrypt the private key if passphrase is provided
            if (!passphrase.IsNullOrEmpty())
            {
                privateKeyBase64 = AESManager.DecryptAES(privateKeyBase64, passphrase);
            }

            // Convert the base64 string to byte array
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

            // Import the private key into the RSA instance
            _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            _verifiedPrivateKey = true;
            return true;
        }
        catch (SAEException)
        {
            throw; // Re-throw SAEException
        }
        catch (Exception ex)
        {
            throw new SAEException("Failed to verify private key.", ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SAEException"></exception>
    public bool VerifyPublicKey()
    {
        _verifiedPublicKey = false;
        try
        {
            if (!File.Exists(PublicKeyPath))
            {
                throw new SAEException("File doesn't exist");
            }
        }
        catch
        {
            throw new SAEException("Program doesn't have sufficient privileges to open private key file.");
        }
        try
        {
            string publicKeyText = string.Empty;
            using (StreamReader sr = new(PublicKeyPath))
            {
                publicKeyText = sr.ReadToEnd();
            }
            _rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKeyText), out _);
            _verifiedPublicKey = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Generate

    public void GenerateKeys(string path, string? passphrase = null)
    {
        if (path.IsNullOrEmpty())
            throw new SAEException("Path was null or empty while trying to generate keys.");
        if (passphrase != null && passphrase.Length < 4)
            throw new SAEException("Passphrase was shorter than 4 letters while trying to generate keys.");

        _rsa = new RSACryptoServiceProvider(2048);

        // Export private key
        byte[] privateKeyBytes = _rsa.ExportRSAPrivateKey();
        string privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

        // Encrypt private key if passphrase is provided
        if (!passphrase.IsNullOrEmpty())
        {
            privateKeyBase64 = AESManager.EncryptAES(privateKeyBase64, passphrase);
        }

        // Export public key
        byte[] publicKeyBytes = _rsa.ExportRSAPublicKey();
        string publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);

        // Save keys to files
        PrivateKeyPath = Path.Combine(path, DEFAULT_PRIVATE_KEY_NAME);
        PublicKeyPath = Path.Combine(path, DEFAULT_PUBLIC_KEY_NAME);

        File.WriteAllText(PrivateKeyPath, privateKeyBase64);
        File.WriteAllText(PublicKeyPath, publicKeyBase64);

    }

    #endregion

    #region Encrypt/Decrypt

    public bool Encrypt(string sourcePath, string encryptedFileDestinationPath)
    {
        try
        {
            if (!IsVerified)
                throw new SAEException("Keys are not verified.");

            byte[] sourceData = File.ReadAllBytes(sourcePath);

            byte[] encryptedData = _rsa.Encrypt(sourceData, FOAEP);

            File.WriteAllBytes(encryptedFileDestinationPath, encryptedData);

            return true;
        }
        catch (Exception ex)
        {
            throw new SAEException("Failed to encrypt file.", ex);
        }
    }

    public bool Decrypt(string sourcePath, string decryptedFileDestinationPath)
    {
        try
        {
            if (!IsVerified)
                throw new SAEException("Keys are not verified.");

            byte[] encryptedData = File.ReadAllBytes(sourcePath);

            byte[] decryptedData = _rsa.Decrypt(encryptedData, FOAEP);

            File.WriteAllBytes(decryptedFileDestinationPath, decryptedData);

            return true;
        }
        catch (Exception ex)
        {
            throw new SAEException("Failed to decrypt file.", ex);
        }
    }

    #endregion

    #endregion

}
