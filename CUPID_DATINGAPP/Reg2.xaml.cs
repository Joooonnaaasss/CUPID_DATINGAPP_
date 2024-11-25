using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CUPID_DATINGAPP
{
    public partial class Reg2 : UserControl
    {
        private Dictionary<string, string> registrationData;

        public Reg2()
        {
            InitializeComponent();
            registrationData = new Dictionary<string, string>();
        }

        public Reg2(Dictionary<string, string> data) : this()
        {
            registrationData = data ?? new Dictionary<string, string>();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                registrationData["Username"] = UsernameTextBox.Text.Trim();
                registrationData["Password"] = PasswordBox.Text.Trim();
                registrationData["TargetAudience"] = ((ComboBoxItem)TargetAudienceComboBox.SelectedItem).Content.ToString();

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.RegistrierFrame.Content = new Reg3(registrationData);
                mainWindow.ShowFrame(mainWindow.RegistrierFrame);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            // RegistrierFrame sichtbar machen
            mainWindow.RegistrierFrame.Visibility = Visibility.Visible;

            // Inhalte setzen
            mainWindow.RegistrierFrame.Content = new Reg(registrationData);

            // ShowFrame-Methode verwenden
            mainWindow.ShowFrame(mainWindow.RegistrierFrame);
        }

        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Bilder (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Wählen Sie ein Profilbild aus"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string photoPath = openFileDialog.FileName;
                registrationData["profile_photo"] = photoPath;
                MessageBox.Show($"Foto erfolgreich hochgeladen: {photoPath}", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Kein Foto ausgewählt.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || UsernameTextBox.Text == "Benutzername")
            {
                MessageBox.Show("Bitte geben Sie einen Benutzernamen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (TargetAudienceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie eine Zielgruppe aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(PasswordBox.Text) || PasswordBox.Text == "Password")
            {
                MessageBox.Show("Bitte geben Sie ein Passwort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Benutzername")
            {
                UsernameTextBox.Text = "";
                UsernameTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "Benutzername";
                UsernameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Text == "Password")
            {
                PasswordBox.Text = "";
                PasswordBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                PasswordBox.Text = "Password";
                PasswordBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
