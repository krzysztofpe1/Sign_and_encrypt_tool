using Microsoft.Win32;
using SignAndEncyptTool.KeysManagement;
using SignAndEncyptTool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsClient.Utils;

namespace WindowsClient.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy GenerateKeyWindow.xaml
    /// </summary>
    public partial class GenerateKeyWindow : Window
    {
        public KeyManager? KeyManager { get; private set; } = null;
        public GenerateKeyWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var path = pathTextBox.Text;
            var passphrase = passphrasePasswordBox.Password;
            passphrase = (passphrase == string.Empty) ? null : passphrase;

            if (path.IsNullOrEmpty())
            {
                MessageBoxes.Warn("Path was unspecified.", "Enter correct path");
                return;
            }

            if (privateKeyEncryptedCheckBox.IsChecked.Value && passphrase.Length < 4)
            {
                MessageBoxes.Warn("Passcode should be at least 4 letters long.", "Passcode too short");
                return;
            }

            try
            {
                var keyManager = new KeyManager();
                keyManager.GenerateKeys(path, passphrase);
                KeyManager = keyManager;
                Close();
            }catch(Exception ex)
            {
                MessageBoxes.Error(ex.Message, "Error while generating keys");
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFolderDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog().Value)
            {
                pathTextBox.Text = ofd.FolderName;
            }
        }
    }
}
