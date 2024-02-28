using System.Diagnostics;
using System.Windows;

namespace WindowsClient;

public partial class MainWindow : Window
{
    private bool _dispose = false;
    public MainWindow()
    {
        var keysWindow = new KeysManagementWindow();
        if (!keysWindow.ShowDialog().Value)
        {
            _dispose = true;
        }
        InitializeComponent();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender == signatureCheckBrowseButton)
        {
            Debug.WriteLine("Check");
        }
        else if (sender == signatureSignBrowseButton)
        {
            Debug.WriteLine("Sign");
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if(_dispose)
            Close();
    }
}