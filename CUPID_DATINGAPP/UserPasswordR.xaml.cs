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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CUPID_DATINGAPP
{
    /// <summary>
    /// Interaction logic for UserPasswordR.xaml
    /// </summary>
    public partial class UserPasswordR : UserControl
    {
        public UserPasswordR()
        {
            InitializeComponent();
        }

        // Methode zur Steuerung der Sichtbarkeit des Platzhalters
        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Zeigt den Platzhalter nur an, wenn das Textfeld leer ist
            PlaceholderTextBlock.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Methode zur Behandlung des Klickereignisses
        private void SendResetLink_Click(object sender, RoutedEventArgs e)
        {
            // Hier kann die Logik zum Senden des Zurücksetzungslinks hinzugefügt werden
            StatusMessage.Text = "Link wurde gesendet!";
            StatusMessage.Visibility = Visibility.Visible;
        }

    }
}
