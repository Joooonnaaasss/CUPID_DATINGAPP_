using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        // Event-Handler: Geburtsdatum speichern
        private void DateOfBirthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        // Event-Handler: Geschlecht speichern
        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        // Event-Handler: Geburtsdatum speichern
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                registrationData["FirstName"] = VornameTextBox.Text.Trim();
                registrationData["LastName"] = NachnameTextBox.Text.Trim();
                registrationData["Email"] = MailTextBox.Text.Trim();

                // Konvertiere das Geburtsdatum ins MySQL-kompatible Format
                DateTime dateOfBirth = (DateTime)DateOfBirthTextBox.SelectedDate;
                registrationData["DateOfBirth"] = dateOfBirth.ToString("yyyy-MM-dd"); // MySQL-Formatierung

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
            if (string.IsNullOrWhiteSpace(VornameTextBox.Text) || VornameTextBox.Text == "Vorname")
            {
                MessageBox.Show("Bitte geben Sie Ihren Vornamen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(NachnameTextBox.Text) || NachnameTextBox.Text == "Nachname")
            {
                MessageBox.Show("Bitte geben Sie Ihren Nachnamen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(MailTextBox.Text) || MailTextBox.Text == "E-Mail" || !IsValidEmail(MailTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (DateOfBirthTextBox.SelectedDate == null)
            {
                MessageBox.Show("Bitte geben Sie ein gültiges Geburtsdatum ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!IsAtLeast18YearsOld((DateTime)DateOfBirthTextBox.SelectedDate))
            {
                MessageBox.Show("Sie müssen mindestens 18 Jahre alt sein, um sich zu registrieren.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (GenderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie Ihr Geschlecht aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsAtLeast18YearsOld(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age >= 18;
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

        private void VornameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (VornameTextBox.Text == "Vorname")
            {
                VornameTextBox.Text = "";
                VornameTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void VornameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VornameTextBox.Text))
            {
                VornameTextBox.Text = "Vorname";
                VornameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void NachnameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NachnameTextBox.Text == "Nachname")
            {
                NachnameTextBox.Text = "";
                NachnameTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void NachnameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NachnameTextBox.Text))
            {
                NachnameTextBox.Text = "Nachname";
                NachnameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void MailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MailTextBox.Text == "E-Mail")
            {
                MailTextBox.Text = "";
                MailTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void MailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MailTextBox.Text))
            {
                MailTextBox.Text = "E-Mail";
                MailTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void VornameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NachnameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
