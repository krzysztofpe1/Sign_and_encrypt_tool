﻿using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using System.Windows;

namespace WindowsClient;

public partial class KeysManagementWindow : Window
{

    #region Private vars

    /// <summary>
    /// Change this field value accordingly: true - correctly parsed one of the keys, false - user closed the window
    /// </summary>
    private bool _returnValue = false;

    #endregion

    public KeysManagementWindow()
    {
        InitializeComponent();
    }

    #region Public Props

    public KeyManager KeyManager { get; private set; } = new();

    #endregion

    #region Public Methods

    public new bool? ShowDialog()
    {
        base.ShowDialog();
        return _returnValue;
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
                            _returnValue = true;
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

    }

    #endregion

}
