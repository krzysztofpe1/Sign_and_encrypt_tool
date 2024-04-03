using SignAndEncyptTool.Utils;
using System.Security.Cryptography;

namespace SignAndEncyptTool.KeysManagement;

public class KeyManager
{

    #region Private default file names

    private const string DEFAULT_PRIVATE_KEY_NAME = "privateKey";
    private const string DEFAULT_PUBLIC_KEY_NAME = "pubKey.pub";
    private const Int32 DEFAULT_KEY_SIZE = 2048;

    #endregion

    #region Private vars

    private string _privateKeyText = string.Empty;
    private string _publicKeyText = string.Empty;
    private bool _verified = false;
    private RSA _rsa = RSA.Create();

    #endregion

    #region Public Props

    private string _privateKeyPath = string.Empty;
    public string PrivateKeyPath
    {
        get { return _privateKeyPath; }
        set
        {
            _privateKeyPath = value;
            _verified = false;
        }
    }

    private string _publicKeyPath = string.Empty;
    public string PublicKeyPath
    {
        get { return _publicKeyPath; }
        set
        {
            _publicKeyPath = value;
            _verified = false;
        }
    }

    public bool IsVerified { get { return _verified; } }

    #endregion

    public KeyManager() { }

    #region Public Methods

    #region Verify

    public bool Verify(string? passphrase = null)
    {
        if (_verified)
            return true;

        if (PublicKeyPath == string.Empty && PrivateKeyPath == string.Empty)
            return _verified = false;

        if (PublicKeyPath != string.Empty)
        {
            if (VerifyPublicKey())
                _verified = true;
            else
                return _verified = false;
        }

        if (PrivateKeyPath != string.Empty)
        {
            if (VerifyPrivateKey(passphrase))
                _verified = true;
            else
                return _verified = false;
        }

        return _verified;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="passphrase"></param>
    /// <returns></returns>
    /// <exception cref="SAEException"></exception>
    public bool VerifyPrivateKey(string? passphrase = null)
    {
        try
        {
            if (!File.Exists(PrivateKeyPath))
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
            using (StreamReader sr = new(PrivateKeyPath))
            {
                _privateKeyText = sr.ReadToEnd();
            }
            if (!passphrase.IsNullOrEmpty())
            {
                _privateKeyText = AESManager.DecryptAES(_privateKeyText, passphrase);
            }
            try
            {
                _rsa.ImportRSAPrivateKey(Convert.FromBase64String(_privateKeyText), out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            throw new SAEException("Program doesn't have sufficient privileges to open private key file.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SAEException"></exception>
    public bool VerifyPublicKey()
    {
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
            using (StreamReader sr = new(PublicKeyPath))
            {
                _publicKeyText = sr.ReadToEnd();
            }
            _rsa.ImportRSAPublicKey(Convert.FromBase64String(_publicKeyText), out _);
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

        using (RSA rsa = RSA.Create(DEFAULT_KEY_SIZE))
        {
            byte[] privateKeyBytes = rsa.ExportRSAPrivateKey();
            string privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

            if (!passphrase.IsNullOrEmpty())
            {
                privateKeyBase64 = AESManager.EncryptAES(privateKeyBase64, passphrase);
            }

            // Export public key
            byte[] publicKeyBytes = rsa.ExportRSAPublicKey();
            string publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);

            // Save keys to files
            PrivateKeyPath = Path.Combine(path, DEFAULT_PRIVATE_KEY_NAME);
            PublicKeyPath = Path.Combine(path, DEFAULT_PUBLIC_KEY_NAME);

            File.WriteAllText(PrivateKeyPath, privateKeyBase64);
            File.WriteAllText(PublicKeyPath, publicKeyBase64);
        }
    }

    #endregion

    #endregion

}
