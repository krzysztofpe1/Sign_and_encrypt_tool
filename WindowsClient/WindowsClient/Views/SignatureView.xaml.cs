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

    }
}
