using System;
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

        // Email Placeholder
        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrWhiteSpace(EmailTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Date Placeholder
        private void DateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatePlaceholder.Visibility = string.IsNullOrWhiteSpace(DateTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Password Placeholder
        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrWhiteSpace(PasswordTextBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Reset Password Logic
        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string date = DateTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(password))
            {
                StatusMessage.Text = "Bitte füllen Sie alle Felder aus.";
                StatusMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                StatusMessage.Visibility = Visibility.Visible;
                return;
            }

            // Hier die Passwort-Zurücksetzen-Logik implementieren
            StatusMessage.Text = "Passwort erfolgreich zurückgesetzt!";
            StatusMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            StatusMessage.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
