using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using SignAndEncyptTool.Utils;
using System.IO;
using System.Windows;
using WindowsClient.Utils;
using WindowsClient.Windows;

namespace WindowsClient;

public partial class KeysManagementWindow : Window
{

    #region Private vars

    /// <summary>
    /// Change this field value accordingly: true - correctly parsed one of the keys, false - user closed the window
    /// </summary>
    private bool _returnValue = false;
    private bool _scheduledToDispose = false;

    #endregion

    public KeysManagementWindow()
    {
        InitializeComponent();
    }

    #region Public Props

    public KeyManager KeyManager { get; private set; } = new();

    #endregion

    #region overriden ShowDialog

    public new bool? ShowDialog()
    {
        base.ShowDialog();
        return _returnValue;
    }

    #endregion

    #region Private Methods

    private void CheckForDriveWithKeys()
    {
        var allDrives = DriveInfo.GetDrives();
        var listOfPrivateAndPublicKeyTuples = new List<(string, string)>();

        foreach (DriveInfo drive in allDrives)
        {
            if (!drive.IsReady)
                continue;

            string privateKeyFilePath = Path.Combine(drive.RootDirectory.FullName, KeyManager.DEFAULT_PRIVATE_KEY_NAME);
            string publicKeyFilePath = Path.Combine(drive.RootDirectory.FullName, KeyManager.DEFAULT_PUBLIC_KEY_NAME);
            var privKeyExists = File.Exists(privateKeyFilePath);
            var pubKeyExists = File.Exists(publicKeyFilePath);

            if (privKeyExists && pubKeyExists)
                listOfPrivateAndPublicKeyTuples.Add((privateKeyFilePath, publicKeyFilePath));
            else if (pubKeyExists)
                listOfPrivateAndPublicKeyTuples.Add((string.Empty, publicKeyFilePath));
            else if (privKeyExists)
                listOfPrivateAndPublicKeyTuples.Add((privateKeyFilePath, string.Empty));
        }

        if (listOfPrivateAndPublicKeyTuples.Count == 0)
            return;

        if (listOfPrivateAndPublicKeyTuples.Count > 1)
        {
            MessageBoxes.Info("Found multiple devices connected with private key and/or public key installed.\nNo action will be performed, choose keys manually.", "Keys scan result");
            return;
        }

        var res = MessageBoxes.YesNoCancel("Private key and/or Public key detected.\nDo you want to import it?", "Keys scan result");
        
        if (res == MessageBoxResult.Yes)
        {
            privateKeyPathTextBox.Text = listOfPrivateAndPublicKeyTuples[0].Item1;
            publicKeyPathTextBox.Text = listOfPrivateAndPublicKeyTuples[0].Item2;
        }
    }

    #endregion

    #region GUI Interactions

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog();
        ofd.Filter = "All files (*.*)|*.*";

        if (!ofd.ShowDialog() == true)
            return;

        if (sender == privateKeyBrowseButton)
        {
            privateKeyPathTextBox.Text = ofd.FileName;
        }
        else if (sender == publicKeyBrowseButton)
        {
            publicKeyPathTextBox.Text = ofd.FileName;
        }
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        var privKeyPath = privateKeyPathTextBox.Text;
        var pubKeyPath = publicKeyPathTextBox.Text;

        if (privKeyPath.IsNullOrEmpty() && pubKeyPath.IsNullOrEmpty())
        {
            MessageBoxes.Error("You have to specify at least one key.", "No key was specified");
        }

        if (privKeyPath != string.Empty)
        {
            KeyManager.PrivateKeyPath = privKeyPath;
            if (privateKeyEncryptedCheckBox.IsChecked.Value)
            {
                SimpleInputWindow siw;
                do
                {
                    siw = new();
                    siw.ShowDialog();
                    if (siw.Input == -1)
                    {
                        _returnValue = false;
                        return;
                    }
                    try
                    {
                        if (KeyManager.VerifyPrivateKey(siw.Input.ToString()))
                        {
                            _returnValue = true;
                            break;
                        }
                        else
                        {
                            _returnValue = false;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxes.Error(ex.Message, "Reading private key error");
                    }
                } while (true);
            }
            else
            {
                try
                {
                    if (KeyManager.VerifyPrivateKey())
                    {
                        _returnValue = true;
                    }
                    else
                    {
                        KeyManager.PrivateKeyPath = string.Empty;
                        _returnValue = false;
                        MessageBoxes.Error("Supplied file is not in valid RSA private key format.", "Public key verification failure");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    KeyManager.PrivateKeyPath = string.Empty;
                    MessageBoxes.Error(ex.Message, "Private key verification failure");
                    return;
                }
            }
        }

        // public key verification
        if (pubKeyPath != string.Empty)
        {
            KeyManager.PublicKeyPath = pubKeyPath;
            try
            {
                if (KeyManager.VerifyPublicKey())
                    _returnValue = true;
                else
                {
                    KeyManager.PublicKeyPath = string.Empty;
                    _returnValue = false;
                    MessageBoxes.Error("Supplied file is not in valid RSA public key format.", "Public key verification failure");
                    return;
                }
            }
            catch (Exception ex)
            {
                KeyManager.PublicKeyPath = string.Empty;
                _returnValue = false;
                MessageBoxes.Error(ex.Message, "Public key verification failure");
                return;
            }
        }
        Close();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new GenerateKeyWindow();
        window.ShowDialog();
        var keyManager = window.KeyManager;
        if (keyManager != null)
        {
            KeyManager = keyManager;
            privateKeyPathTextBox.Text = keyManager.PrivateKeyPath;
            publicKeyPathTextBox.Text = keyManager.PublicKeyPath;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CheckForDriveWithKeys();
    }

    #endregion

}
