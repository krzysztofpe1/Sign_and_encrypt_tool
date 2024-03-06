using SignAndEncyptTool.KeysManagement;
using System.Windows.Controls;

namespace WindowsClient.Views;

public partial class EncryptionView : UserControl
{
    private readonly KeyManager _keyManager;

    public EncryptionView(KeyManager keyManager)
    {
        InitializeComponent();
        _keyManager = keyManager;
    }
}
