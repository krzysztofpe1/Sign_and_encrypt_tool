using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using SignAndEncyptTool.Utils;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WindowsClient.Utils;

namespace WindowsClient.Views;

public partial class EncryptionView : UserControl
{
    private readonly KeyManager _keyManager;

    public EncryptionView(KeyManager keyManager)
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

        pathTextBox.Text = ofd.FileName;
    }

    private void EncryptButton_Click(object sender, RoutedEventArgs e)
    {
        var mbResult = MessageBoxes.YesNoCancel("Do you want to replace the source file with an encrypted file?", "Replace source file");

        if (mbResult == MessageBoxResult.Cancel)
            return;

        var path = pathTextBox.Text;

        if (mbResult == MessageBoxResult.No)
        {
            var sfd = new SaveFileDialog();
            string filter = "Encrypted file .enc|*.enc";
            //Add custom file extension
            var originalExtenstion = Path.GetExtension(pathTextBox.Text);
            if (!originalExtenstion.IsNullOrEmpty())
                filter += $"|Encrypted file *{originalExtenstion}|*{originalExtenstion}";
            sfd.Filter = filter;
            sfd.FilterIndex = 0;
            if (sfd.ShowDialog().Value)
                path = sfd.FileName + KeyManager.DEFAULT_ENCRYPTED_FILE_EXTENSION;
            else
            {
                MessageBoxes.Info("Encryption process was canceled.\nNo changes were made to the source file and the encrypted file was not generated.", "Encryption canceled");
                return;
            }
        }
        //mbResult is equal "Yes"
        try
        {
            _keyManager.Encrypt(pathTextBox.Text, path);
            MessageBoxes.Info($"File was successfully encrypted and saved to location: {path}", "Encryption successfull");
        }
        catch (Exception ex)
        {
            MessageBoxes.Error($"An exception was thrown while trying to encypt and save the file.\nException thrown:\n{ex.Message}", "Encryption error");
        }
    }
}
