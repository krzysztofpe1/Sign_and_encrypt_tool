using Microsoft.Win32;
using System.Windows;

namespace WindowsClient;

public partial class KeysManagementWindow : Window
{
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
        SimpleInputWindow siw;
        do
        {
            siw = new();
            siw.ShowDialog();
            if (siw.Input == -1)
            {
                return;
            }
        } while (true);
        if (sender == privateKeyBrowseButton)
        {

        }
        else if(sender == publicKeyBrowseButton)
        {

        }
    }
}
