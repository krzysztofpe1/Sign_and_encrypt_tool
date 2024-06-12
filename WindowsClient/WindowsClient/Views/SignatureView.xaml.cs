using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using SignAndEncyptTool.Utils;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WindowsClient.Utils;

namespace WindowsClient.Views;

public partial class SignatureView : UserControl
{
    private readonly KeyManager _keyManager;

    public SignatureView(KeyManager keyManager)
    {
        InitializeComponent();
        _keyManager = keyManager;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog();
        ofd.Filter = "All files (*.*)|*.*";

        if (!ofd.ShowDialog() == true)
            return;

        if (sender == signatureCheckBrowseButton)
        {
            signatureCheckPathTextBox.Text = ofd.FileName;
        }
        else if (sender == signatureSignBrowseButton)
        {
            signatureSignPathTextBox.Text = ofd.FileName;
        }
    }

    private async void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        var documentPath = signatureCheckPathTextBox.Text;
        var signaturePath = _keyManager.GetDefaultSignaturePath(documentPath);
        if (!File.Exists(signaturePath))
        {
            MessageBoxes.Warn("Programm couldn't find signature file.\nClick Ok and select the signature file.", "Signature not found");
            var ofd = new OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*|XML files (*.xml)|*.xml";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();
            if (ofd.ShowDialog() == false)
                return;
            signaturePath = ofd.FileName;
        }
        else
            MessageBoxes.Info("Proceeding to verify signature.", "Signature found.");

        if (await _keyManager.VerifySignature(documentPath, signaturePath))
            MessageBoxes.Info("Signature verified succesfully!", "Signature verification status");
        else
            MessageBoxes.Error("Signature verification failure.\nFile or signature were modified!", "Signature verification status");

    }

    private async void SignButton_Click(object sender, RoutedEventArgs e)
    {
        var documentPath = signatureSignPathTextBox.Text;
        if (await _keyManager.SignDocument(documentPath))
            MessageBoxes.Info("Signature created succesfully.", "Signature creation status");
        else
            MessageBoxes.Error("Signature couldn't be created.\nCheck permissions to the folder containing file to be signed.", "Signature creation status");
    }

    private void UC_Loaded(object sender, RoutedEventArgs e)
    {
        if (_keyManager.PrivateKeyPath.IsNullOrEmpty())
        {
            signatureSignBrowseButton.IsEnabled = false;
            signatureSignPathTextBox.IsEnabled = false;
            signButton.IsEnabled = false;
        }
        if (_keyManager.PublicKeyPath.IsNullOrEmpty())
        {
            signatureCheckBrowseButton.IsEnabled = false;
            signatureCheckPathTextBox.IsEnabled = false;
            checkButton.IsEnabled = false;
        }
    }
}
