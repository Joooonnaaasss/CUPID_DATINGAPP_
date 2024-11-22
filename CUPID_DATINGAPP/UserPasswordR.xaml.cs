using System.Windows;
using System.Windows.Controls;

namespace CUPID_DATINGAPP
{
    public partial class UserPasswordR : UserControl
    {
        public UserPasswordR()
        {
            InitializeComponent();
        }

        // Eventhandler für die E-Mail-Textbox
        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                EmailPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                EmailPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        // Eventhandler für das Datum-Textbox
        private void DateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DateTextBox.Text))
            {
                DatePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                DatePlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        // Eventhandler für die Passwort-Textbox
        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        // Schließen-Button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Logik zum Schließen des Fensters oder der App
        }

        // Passwort zurücksetzen Button
        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            // Logik zum Zurücksetzen des Passworts
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(DateTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                StatusMessage.Text = "Bitte füllen Sie alle Felder aus.";
                StatusMessage.Visibility = Visibility.Visible;
            }
            else
            {
                StatusMessage.Text = "Passwort erfolgreich zurückgesetzt.";
                StatusMessage.Visibility = Visibility.Visible;

                // Weitere Logik hinzufügen, um das Passwort tatsächlich zurückzusetzen
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            // Lade die Login-Seite in den LogFrame
            mainWindow.SettingsFrame.Content = new UserSettings(); // Log ist die Login-Oberfläche

            mainWindow.ShowFramesWithoutHidingMenu(mainWindow.SettingsFrame);
        }

    }
}
