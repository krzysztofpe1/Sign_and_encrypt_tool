using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using System.Windows;
using System.Windows.Controls;
using WindowsClient.Utils;

namespace WindowsClient.Views;

public partial class DecryptionView : UserControl
{
    private readonly KeyManager _keyManager;

    public DecryptionView(KeyManager keyManager)
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

    private void DecryptButton_Click(object sender, RoutedEventArgs e)
    {
        var mbResult = MessageBoxes.YesNoCancel("Do you want to replace the source file with a decrypted file?", "Replace source file");
        var path = pathTextBox.Text;
        if (mbResult == MessageBoxResult.Cancel)
            return;
        if (mbResult == MessageBoxResult.Yes)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Encrypted file .enc|*.enc";
            if (sfd.ShowDialog().Value)
                path = sfd.FileName;
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
