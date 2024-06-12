using SignAndEncyptTool.KeysManagement;
using SignAndEncyptTool.Utils;
using System.Windows;
using WindowsClient.Views;

namespace WindowsClient;

public partial class MainWindow : Window
{
    private bool _dispose = false;
    private KeyManager _keyManager;

    public MainWindow()
    {
        var keysWindow = new KeysManagementWindow();
        if (!keysWindow.ShowDialog().Value)
        {
            _dispose = true;
        }
        _keyManager = keysWindow.KeyManager;
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_dispose)
            Close();
        else
        {
            signatureContentControl.Content = new SignatureView(_keyManager);
            if (!_keyManager.PublicKeyPath.IsNullOrEmpty())
                encryptionContentControl.Content = new EncryptionView(_keyManager);
            else
                encryptionTab.IsEnabled = false;
            
            if (!_keyManager.PrivateKeyPath.IsNullOrEmpty())
                decryptionContentControl.Content = new DecryptionView(_keyManager);
            else 
                decryptionTab.IsEnabled = false;
        }
    }
}