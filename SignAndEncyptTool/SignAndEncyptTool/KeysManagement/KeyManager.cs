using System.Security.Cryptography;

namespace SignAndEncyptTool.KeysManagement;

public class KeyManager
{
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
            if (passphrase != null && passphrase != string.Empty)
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

}
