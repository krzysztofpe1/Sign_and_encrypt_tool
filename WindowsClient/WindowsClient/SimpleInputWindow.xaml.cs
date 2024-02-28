using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace WindowsClient;

public partial class SimpleInputWindow : Window
{
    /// <summary>
    /// -1 if window was closed, else value entered in textbox
    /// </summary>
    public int Input { get; private set; } = -1;
    public SimpleInputWindow()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if(inputTextBox.Text.Length < 4)
        {
            MessageBox.Show("Required at least 4 digits.");
            return;
        }
        try
        {
            Input = int.Parse(inputTextBox.Text);
        }
        catch
        {
            Input = 0;
        }
        Close();
    }

    #region Numeric field GUI interaction

    private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void NumericTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if(e.Key == System.Windows.Input.Key.Space)
            e.Handled = true;
    }

    #endregion
}
