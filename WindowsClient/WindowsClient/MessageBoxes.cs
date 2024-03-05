using System.Windows;

namespace WindowsClient;

internal static class MessageBoxes
{
    public static void Info(string message, string caption)
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public static void Warn(string message, string caption)
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public static void Error(string message, string caption)
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
