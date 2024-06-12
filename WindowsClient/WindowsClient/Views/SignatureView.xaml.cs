using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using System.Windows.Controls;

namespace WindowsClient.Views;

public partial class SignatureView : UserControl
{
    private readonly KeyManager _keyManager;

    public SignatureView(KeyManager keyManager)
    {
        InitializeComponent();
        _keyManager = keyManager;
    }

    private void BrowseButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog();
        ofd.Filter = "All files (*.*)|*.*";

        if (!ofd.ShowDialog() == true)
            return;

        if (sender == checkButton)
        {
            signatureCheckPathTextBox.Text = ofd.FileName;
        }
        else if (sender == signButton)
        {
            signatureSignPathTextBox.Text = ofd.FileName;
        }
    }

    private void CheckButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }

    private void SignButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}
