using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CUPID_DATINGAPP
{
    public partial class Reg : UserControl
    {
        private Dictionary<string, string> registrationData = new Dictionary<string, string>();


        public Reg()
        {
            InitializeComponent();
            registrationData = new Dictionary<string, string>();
        }

        // Konstruktor mit Übergabe eines Dictionaries
        public Reg(Dictionary<string, string> data)
        {
            registrationData = data;
        }


        // Event-Handler: Vorname speichern
        private void VornameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        // Event-Handler: Nachname speichern
        private void NachnameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        // Event-Handler: E-Mail speichern
        private void MailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        // Event-Handler: Geburtsdatum speichern
        private void DateOfBirthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        // Event-Handler: Geschlecht speichern
        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        // Weiterleitung zu Reg2
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                registrationData["FirstName"] = VornameTextBox.Text.Trim();
                registrationData["LastName"] = NachnameTextBox.Text.Trim();
                registrationData["Email"] = MailTextBox.Text.Trim();
                registrationData["DateOfBirth"] = DateOfBirthTextBox.Text.Trim();
                registrationData["Gender"] = GenderComboBox.Text.Trim();
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                // Lade Reg2 in den RegistrierFrame
                mainWindow.RegistrierFrame.Content = new Reg2(registrationData); // Reg2 als Inhalt setzen

                // Zeige den RegistrierFrame an
                mainWindow.ShowFrame(mainWindow.RegistrierFrame);
            }
        }

        // Zurück zum Login
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            // Lade die Login-Seite in den LogFrame
            mainWindow.LogFrame.Content = new Log(); // Log ist die Login-Oberfläche

            // Zeige den LogFrame an
            mainWindow.ShowFrame(mainWindow.LogFrame);
        }

        // Eingabevalidierung
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(VornameTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie Ihren Vornamen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(NachnameTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie Ihren Nachnamen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(MailTextBox.Text) || !IsValidEmail(MailTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(DateOfBirthTextBox.Text) || !DateTime.TryParse(DateOfBirthTextBox.Text, out _))
            {
                MessageBox.Show("Bitte geben Sie ein gültiges Geburtsdatum ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (GenderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie Ihr Geschlecht aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }


        }
    }
}
