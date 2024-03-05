using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace WindowsClient;

public partial class KeysManagementWindow : Window
{
    /// <summary>
    /// Change this field value accordingly: true - correctly parsed one of the keys, false - user closed the window
    /// </summary>
    private bool _returnValue = false;
    public KeysManagementWindow()
    {
        InitializeComponent();
    }

    public new bool? ShowDialog()
    {
        base.ShowDialog();
        return _returnValue;
    }

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
        if (privKeyPath == string.Empty || pubKeyPath == string.Empty)
        {
            MessageBoxes.Error("You have to specify path for one of keys.", "Invalid keys path");
        }

        if (privKeyPath != string.Empty)
        {
            try
            {
                if (!File.Exists(privKeyPath))
                {
                    MessageBoxes.Error("File under specified path doesn't exist.", "Invalid private key path");
                }
            }
            catch
            {
                MessageBoxes.Error("Program doesn't have sufficient privileges to open private key file.", "Invalid private key path");
            }
            string privateKeyText = string.Empty;

            //Private Key is encrypted
            if (privateKeyEncryptedCheckBox.IsChecked.Value)
            {

                SimpleInputWindow siw;
                do
                {
                    siw = new();
                    siw.ShowDialog();
                    if (siw.Input == -1)
                    {
                        return;
                    }
                    try
                    {
                        using (StreamReader sr = new(privKeyPath))
                        {
                            string encryptedText = sr.ReadToEnd();
                            //decrypting the key
                            string decryptedText;

                            using RSA rsa = RSA.Create();
                            rsa.ImportRSAPrivateKey(Convert.FromBase64String(encryptedText), out _);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxes.Error(ex.Message, "Reading private key error");
                    }
                } while (true);
            }
            //Private Key is not encrypted
            else
            {
                try
                {
                    using (StreamReader sr = new(privKeyPath))
                    {
                        string keyText = sr.ReadToEnd();

                        using RSA rsa = RSA.Create();
                        rsa.ImportRSAPrivateKey(Convert.FromBase64String(keyText), out _);
                        _returnValue = true;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxes.Error(ex.Message, "Reading private key error");
                }
            }

        }
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
