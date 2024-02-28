using System.Diagnostics;
using System.Windows;

namespace WindowsClient;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        if(sender == signatureCheckBrowseButton)
        {
            Debug.WriteLine("Check");
        }
        else if(sender == signatureSignBrowseButton)
        {
            Debug.WriteLine("Sign");
        }
    }
}